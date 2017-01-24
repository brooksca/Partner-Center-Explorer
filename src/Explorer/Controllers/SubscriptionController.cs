// -----------------------------------------------------------------------
// <copyright file="SubscriptionController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using Filters.WebApi;
    using Logic;
    using Logic.Authentication;
    using Models;
    using PartnerCenter.Models.Customers;
    using PartnerCenter.Models.Subscriptions;
    using RequestContext;

    /// <summary>
    /// Provides the ability to manage subscriptions.
    /// </summary>
    [RoutePrefix("api/subscription")]
    public class SubscriptionController : BaseApiController
    {
        /// <summary>
        /// Provides access to the appropriate subscription monitor service. 
        /// </summary>
        private ISubscriptionMonitor monitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionController" /> class.
        /// </summary>
        /// <param name="service">Provides access to the core application services.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public SubscriptionController(IExplorerService service) : base(service)
        {
        }

        /// <summary>
        /// Gets a summary for the specified subscription.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="subscriptionId">Identifier for the subscription.</param>
        /// <returns>A summary for the specified subscription.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="subscriptionId"/> is empty or null.
        /// </exception>
        [@Authorize(UserRole = UserRole.Any)]
        [HttpGet]
        [Route("summary")]
        public async Task<SubscriptionSummaryViewModel> GetSubscriptionSummaryAsync(string customerId, string subscriptionId)
        {
            CultureInfo responseCulture;
            Customer customer;
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMeasurements;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            List<IncidentModel> incidents;
            Subscription subscription;

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                principal = (CustomerPrincipal)HttpContext.Current.User;

                if (string.IsNullOrEmpty(customerId) || !principal.IsAdmin)
                {
                    customerId = principal.CustomerId;
                }

                subscriptionId.AssertNotEmpty(nameof(subscriptionId));

                responseCulture = new CultureInfo(Services.Localization().Locale);

                operations =
                    Services.PartnerCenter.With(RequestContextFactory.Instance.Create(
                        correlationId, responseCulture.Name));

                // Obtain a reference to the customer who owns the subscription(s).
                customer = await operations.Customers.ById(customerId).GetAsync();

                // Obtain to the subscription.
                subscription = await operations.Customers.ById(customerId)
                    .Subscriptions.ById(subscriptionId).GetAsync();

                incidents = await this.GetSubscriptionMonitor(subscription.BillingType, customerId, subscriptionId).GetIncidentsAsync();

                // Track the event measurements for analysis.
                eventMeasurements = new Dictionary<string, double>()
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfActiveIncidents", incidents.Count }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>()
                {
                    { "CustomerId", customerId },
                    { "Email", principal.Email },
                    { "IsAdmin", principal.IsAdmin.ToString() },
                    { "IsCustomer", principal.IsCustomer.ToString() },
                    { "PrincipalCustomerId", principal.CustomerId },
                    { "RequestCorrelationId", correlationId.ToString() },
                    { "SubscriptionId", subscriptionId }
                };

                Services.Telemetry.TrackEvent("/api/subscription/summary", eventProperties, eventMeasurements);

                return new SubscriptionSummaryViewModel()
                {
                    CompanyName = customer.CompanyProfile.CompanyName,
                    CustomerId = customerId,
                    FriendlyName = subscription.FriendlyName, 
                    Incidents = incidents,
                    SubscriptionId = subscriptionId
                };
            }
            finally
            {
                customer = null;
                eventMeasurements = null;
                eventProperties = null;
                incidents = null;
                operations = null;
                principal = null;
                responseCulture = null;
                subscription = null;
            }
        }

        /// <summary>
        /// Gets the appropriate subscription monitor for the specified subscription.
        /// </summary>
        /// <param name="billingType">The way billing is handled for the subscription.</param>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="subscriptionId">Identifier for the subscription.</param>
        /// <returns>An aptly configured instance of <see cref="ISubscriptionMonitor"/>.</returns>
        private ISubscriptionMonitor GetSubscriptionMonitor(PartnerCenter.Models.Invoices.BillingType billingType, string customerId, string subscriptionId)
        {
            customerId.AssertNotEmpty(nameof(customerId));
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            if (this.monitor == null)
            {
                if (billingType == PartnerCenter.Models.Invoices.BillingType.License)
                {
                    this.monitor = new Logic.Office.SubscriptionMonitor(Services, customerId, subscriptionId);
                }
                else
                {
                    this.monitor = new Logic.Azure.SubscriptionMonitor(Services, customerId, subscriptionId);
                }
            }

            return this.monitor;
        }
    }
}