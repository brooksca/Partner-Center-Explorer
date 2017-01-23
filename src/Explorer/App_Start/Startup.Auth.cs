// -----------------------------------------------------------------------
// <copyright file="Startup.Auth.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Web;
    using Azure.ActiveDirectory.GraphClient;
    using Configuration;
    using Exceptions;
    using global::Owin;
    using Logic;
    using Logic.Authentication;
    using Logic.Azure;
    using Owin.Security;
    using Owin.Security.Cookies;
    using Owin.Security.OpenIdConnect;
    using PartnerCenter.Models.Customers;
    using Practices.Unity;
    
    /// <summary>
    /// Provides methods and properties used to start the application.
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Configures authentication for the application.
        /// </summary>
        /// <param name="app">The application to be configured.</param>
        public void ConfigureAuth(IAppBuilder app)
        {
            IExplorerService service = MvcApplication.UnityContainer.Resolve<IExplorerService>();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions { });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = ApplicationConfiguration.ApplicationId,
                    Authority = $"{ApplicationConfiguration.ActiveDirectoryEndpoint}/common",

                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        AuthenticationFailed = (context) =>
                        {
                            // Track the exceptions using the telemetry provider.
                            service.Telemetry.TrackException(context.Exception);

                            // Pass in the context back to the app
                            context.OwinContext.Response.Redirect("/Home/Error");

                            // Suppress the exception
                            context.HandleResponse();

                            return Task.FromResult(0);
                        },
                        AuthorizationCodeReceived = async (context) =>
                        {
                            string userTenantId = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                            string signedInUserObjectId = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

                            IActiveDirectory ad = new ActiveDirectory(
                                service,
                                userTenantId,
                                context.Code,
                                signedInUserObjectId,
                                new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)));

                            List<IDirectoryRole> roles = await ad.GetDirectoryRolesAsync(signedInUserObjectId);

                            foreach (IDirectoryRole role in roles)
                            {
                                context.AuthenticationTicket.Identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role.DisplayName));
                            }

                            if (!userTenantId.Equals(ApplicationConfiguration.ApplicationTenantId, StringComparison.CurrentCultureIgnoreCase))
                            {
                                string customerId = string.Empty;

                                try
                                {
                                    IAggregatePartner partner = PartnerService.Instance.CreatePartnerOperations(
                                        new TokenManagement(service).GetPartnerCenterAppOnlyCredentials(
                                            $"{ApplicationConfiguration.ActiveDirectoryEndpoint}/{ApplicationConfiguration.ApplicationTenantId}"));

                                    Customer c = await partner.Customers.ById(userTenantId).GetAsync();

                                    customerId = c.Id;
                                }
                                catch (PartnerException ex)
                                {
                                    if (ex.ErrorCategory != PartnerErrorCategory.NotFound)
                                    {
                                        throw;
                                    }
                                }

                                if (!string.IsNullOrWhiteSpace(customerId))
                                {
                                    // Add the customer identifier to the claims
                                    context.AuthenticationTicket.Identity.AddClaim(new System.Security.Claims.Claim("CustomerId", customerId));
                                }
                            }
                            else
                            {
                                if (context.AuthenticationTicket.Identity.FindFirst(System.Security.Claims.ClaimTypes.Role).Value != "Company Administrator")
                                {
                                    // this login came from the partner's tenant, only allow admins to access the site, non admins will only
                                    // see the unauthenticated experience but they can't configure the portal nor can purchase
                                    Trace.TraceInformation($"Blocked log in from non admin partner user: {signedInUserObjectId}");

                                    throw new AuthorizationException(System.Net.HttpStatusCode.Unauthorized, "You must have global admin rights.");
                                }
                            }
                        },
                        RedirectToIdentityProvider = (context) =>
                        {
                            string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;
                            context.ProtocolMessage.RedirectUri = appBaseUrl + "/";
                            context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;
                            return Task.FromResult(0);
                        }
                    },
                    TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                    {
                        SaveSigninToken = true,
                        ValidateIssuer = false
                    }
                });
        }
    }
}