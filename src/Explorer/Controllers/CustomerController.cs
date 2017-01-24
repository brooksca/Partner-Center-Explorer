// -----------------------------------------------------------------------
// <copyright file="CustomerController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using Configuration;
    using Filters.WebApi;
    using Logic;
    using Logic.Authentication;
    using Models;
    using Newtonsoft.Json;
    using PartnerCenter.Models;
    using PartnerCenter.Models.Customers;
    using PartnerCenter.Models.Subscriptions;
    using RequestContext;

    /// <summary>
    /// Provides the ability to manage customers.
    /// </summary>
    [RoutePrefix("api/customer")]
    public class CustomerController : BaseApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerController"/> class.
        /// </summary>
        /// <param name="service">Provides access to the core application services.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public CustomerController(IExplorerService service) : base(service)
        {
        }

        /// <summary>
        /// Creates the specified customer.
        /// </summary>
        /// <param name="customerViewModel">Information pertaining to the new customer.</param>
        /// <returns>An object that represents the newly created customer.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="customerViewModel"/> is null.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The authenticated must be a partner administrator to perform this operation.
        /// </exception>
        [@Authorize(UserRole = UserRole.Partner)]
        [HttpPost]
        [Route("")]
        public async Task<CustomerViewModel> CreateAsync([FromBody] CustomerViewModel customerViewModel)
        {
            PartnerCenter.Models.CountryValidationRules.CountryValidationRules customerCountryValidationRules;
            Customer newCustomer;
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMeasurements;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            bool isDomainAvailable;
            string billingCulture;
            string billingLanguage;
            string domainName;

            customerViewModel.AssertNotNull(nameof(customerViewModel));

            if (!ModelState.IsValid)
            {
                var errorList = (from item in ModelState.Values
                                 from error in item.Errors
                                 select error.ErrorMessage).ToList();

                throw new PartnerDomainException(
                    ErrorCode.InvalidInput).AddDetail("ErrorMessage", JsonConvert.SerializeObject(errorList));
            }

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();

                operations =
                    Services.PartnerCenter.With(RequestContextFactory.Instance.Create(
                        correlationId));

                principal = (CustomerPrincipal)HttpContext.Current.User;

                if (!principal.IsAdmin)
                {
                    throw new UnauthorizedAccessException(Resources.UserUnauthorizedException);
                }

                // Get the locale, we default to the first locale used in a country for now.
                customerCountryValidationRules =
                    await operations.CountryValidationRules.ByCountry(customerViewModel.Country).GetAsync();

                billingCulture = customerCountryValidationRules.DefaultCulture;
                billingLanguage = customerCountryValidationRules.SupportedLanguagesList.FirstOrDefault();

                domainName = string.Format(CultureInfo.InvariantCulture, "{0}.onmicrosoft.com", customerViewModel.DomainPrefix);

                // Verify that the domain is available. 
                isDomainAvailable = await operations.Domains.ByDomain(domainName).ExistsAsync();

                if (isDomainAvailable)
                {
                    throw new PartnerDomainException(ErrorCode.DomainNotAvailable).AddDetail("DomainPrefix", domainName);
                }

                newCustomer = new Customer
                {
                    BillingProfile = new CustomerBillingProfile
                    {
                        CompanyName = customerViewModel.CompanyName,
                        Culture = billingCulture,
                        DefaultAddress = new Address
                        {
                            FirstName = customerViewModel.FirstName,
                            LastName = customerViewModel.LastName,
                            AddressLine1 = customerViewModel.AddressLine1,
                            AddressLine2 = customerViewModel.AddressLine2,
                            City = customerViewModel.City,
                            State = customerViewModel.State,
                            Country = customerViewModel.Country,
                            PostalCode = customerViewModel.ZipCode,
                            PhoneNumber = customerViewModel.Phone,
                        },
                        Email = customerViewModel.Email,
                        Language = billingLanguage,
                    },
                    CompanyProfile = new CustomerCompanyProfile
                    {
                        Domain = domainName,
                    },
                };

                newCustomer = await operations.Customers.CreateAsync(newCustomer);

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "Email", principal.Email },
                    { "IsAdmin", principal.IsAdmin.ToString() },
                    { "IsCustomer", principal.IsCustomer.ToString() },
                    { "NewCustomerName", newCustomer.CompanyProfile.CompanyName },
                    { "NewCustomerMicrosoftId", newCustomer.Id },
                    { "PrincipalCustomerId", principal.CustomerId },
                    { "RequestCorrelationId", correlationId.ToString() }
                };

                // Track the event measurements for analysis.
                eventMeasurements = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                Services.Telemetry.TrackEvent("/api/customer/create", eventProperties, eventMeasurements);

                return new CustomerViewModel()
                {
                    AddressLine1 = newCustomer.BillingProfile.DefaultAddress.AddressLine1,
                    AddressLine2 = newCustomer.BillingProfile.DefaultAddress.AddressLine2,
                    AdminUserAccount = $"{newCustomer.UserCredentials.UserName}@{newCustomer.CompanyProfile.Domain}",
                    City = newCustomer.BillingProfile.DefaultAddress.City,
                    CompanyName = newCustomer.BillingProfile.CompanyName,
                    Country = newCustomer.BillingProfile.DefaultAddress.Country,
                    Email = newCustomer.BillingProfile.Email,
                    FirstName = newCustomer.BillingProfile.DefaultAddress.FirstName,
                    Language = newCustomer.BillingProfile.Language,
                    LastName = newCustomer.BillingProfile.DefaultAddress.LastName,
                    MicrosoftId = newCustomer.CompanyProfile.TenantId,
                    Password = newCustomer.UserCredentials.Password,
                    Phone = newCustomer.BillingProfile.DefaultAddress.PhoneNumber,
                    State = newCustomer.BillingProfile.DefaultAddress.State,
                    Username = customerViewModel.Email,
                    ZipCode = newCustomer.BillingProfile.DefaultAddress.PostalCode
                };
            }
            finally
            {
                customerCountryValidationRules = null;
                eventMeasurements = null;
                eventProperties = null;
                newCustomer = null;
                operations = null;
                principal = null;
            }
        }

        /// <summary>
        /// Deletes the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer to be deleted.</param>
        /// <returns>No content is returned.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Delete operations are only valid when connected to integration sandbox.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The authenticated user is not associated with the partner tenant.
        /// </exception>
        [@Authorize(UserRole = UserRole.Partner)]
        [HttpDelete]
        [Route("delete")]
        public async Task<HttpResponseMessage> DeleteAsync(string customerId)
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMeasurements;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;

            customerId.AssertNotEmpty(nameof(customerId));

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();

                operations =
                    Services.PartnerCenter.With(RequestContextFactory.Instance.Create(
                        correlationId));

                principal = (CustomerPrincipal)HttpContext.Current.User;

                if (!ApplicationConfiguration.IsSandboxEnvironment)
                {
                    throw new InvalidOperationException(Resources.CustomerDeleteInvalidException);
                }

                if (principal.IsCustomer)
                {
                    throw new UnauthorizedAccessException(Resources.UserUnauthorizedException);
                }

                await operations.Customers.ById(customerId).DeleteAsync();

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "Email", principal.Email },
                    { "IsAdmin", principal.IsAdmin.ToString() },
                    { "IsCustomer", principal.IsCustomer.ToString() },
                    { "PrincipalCustomerId", principal.CustomerId },
                    { "RequestCorrelationId", correlationId.ToString() }
                };

                // Track the event measurements for analysis.
                eventMeasurements = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                Services.Telemetry.TrackEvent("/api/customer/delete", eventProperties, eventMeasurements);

                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            finally
            {
                eventMeasurements = null;
                eventProperties = null;
                operations = null;
                principal = null;
            }
        }

        /// <summary>
        /// Gets a summary for the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer</param>
        /// <returns>A summary for the given customer.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        [@Authorize(UserRole = UserRole.Any)]
        [HttpGet]
        [Route("summary")]
        public async Task<CustomerSummaryViewModel> GetCustomerSummaryAsync(string customerId)
        {
            CultureInfo responseCulture;
            Customer customer;
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMeasurements;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            ResourceCollection<Subscription> subscriptions;

            customerId.AssertNotEmpty(nameof(customerId));

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();

                principal = (CustomerPrincipal)HttpContext.Current.User;
                responseCulture = new CultureInfo(Services.Localization().Locale);

                operations =
                    Services.PartnerCenter.With(RequestContextFactory.Instance.Create(
                        correlationId, responseCulture.Name));

                if (principal.IsAdmin)
                {
                    customer = await operations.Customers.ById(customerId).GetAsync();
                    subscriptions = await operations.Customers.ById(customerId).Subscriptions.GetAsync();
                }
                else
                {
                    customer = await operations.Customers.ById(principal.CustomerId).GetAsync();
                    subscriptions = await operations.Customers.ById(principal.CustomerId).Subscriptions.GetAsync();
                }

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", customerId },
                    { "Email", principal.Email },
                    { "IsAdmin", principal.IsAdmin.ToString() },
                    { "IsCustomer", principal.IsCustomer.ToString() },
                    { "PrincipalCustomerId", principal.CustomerId },
                    { "RequestCorrelationId", correlationId.ToString() }
                };

                // Track the event measurements for analysis.
                eventMeasurements = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                Services.Telemetry.TrackEvent("/api/customer/summary", eventProperties, eventMeasurements);

                return new CustomerSummaryViewModel()
                {
                    BillingProfile = customer.BillingProfile,
                    CommerceId = customer.CommerceId,
                    CompanyProfile = customer.CompanyProfile,
                    Id = customer.Id,
                    Subscriptions = subscriptions.Items.Where(s => s.Status != SubscriptionStatus.Deleted).Select(s => new SubscriptionViewModel()
                    {
                        BillingType = s.BillingType,
                        CompanyName = customer.CompanyProfile.CompanyName,
                        CustomerId = customerId,
                        FriendlyName = s.FriendlyName,
                        OfferName = s.OfferName,
                        Quantity = s.Quantity,
                        Status = s.Status,
                        SubscriptionId = s.Id,
                        UnitType = s.UnitType
                    }).ToList()
                };
            }
            finally
            {
                customer = null;
                eventMeasurements = null;
                eventProperties = null;
                operations = null;
                principal = null;
                responseCulture = null;
                subscriptions = null;
            }
        }

        /// <summary>
        /// Gets a list of all customers.
        /// </summary>
        /// <returns>
        /// A list of customers utilized for rendering purpose.
        /// </returns>
        /// <remarks>
        /// If the authenticated users is a customer administrator then the only 
        /// customer in the list will be the one they are associated with. This 
        /// is done for security purposes. 
        /// </remarks>
        [@Authorize(UserRole = UserRole.Any)]
        [HttpGet]
        [Route("")]
        public async Task<CustomersViewModel> GetCustomersAsync()
        {
            CultureInfo responseCulture;
            Customer customer;
            CustomerPrincipal principal;
            CustomersViewModel viewModel;
            DateTime startTime;
            Dictionary<string, double> eventMeasurements;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            SeekBasedResourceCollection<Customer> customers;

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();

                principal = (CustomerPrincipal)HttpContext.Current.User;
                responseCulture = new CultureInfo(Services.Localization().Locale);

                operations =
                    Services.PartnerCenter.With(RequestContextFactory.Instance.Create(
                        correlationId, responseCulture.Name));

                viewModel = new CustomersViewModel()
                {
                    Customers = new List<Customer>()
                };

                if (principal.IsAdmin)
                {
                    customers = await operations.Customers.GetAsync();
                    viewModel.Customers.AddRange(customers.Items);
                }
                else
                {
                    customer = await operations.Customers.ById(principal.CustomerId).GetAsync();
                    viewModel.Customers.Add(customer);
                }

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "Email", principal.Email },
                    { "IsAdmin", principal.IsAdmin.ToString() },
                    { "IsCustomer", principal.IsCustomer.ToString() },
                    { "PrincipalCustomerId", principal.CustomerId },
                    { "RequestCorrelationId", correlationId.ToString() }
                };

                // Track the event measurements for analysis.
                eventMeasurements = new Dictionary<string, double>
                {
                    { "CustomerCount", viewModel.Customers.Count },
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                Services.Telemetry.TrackEvent("/api/customer", eventProperties, eventMeasurements);

                return viewModel;
            }
            finally
            {
                customer = null;
                customers = null;
                eventMeasurements = null;
                eventProperties = null;
                operations = null;
                principal = null;
                responseCulture = null;
            }
        }
    }
}