// -----------------------------------------------------------------------
// <copyright file="SubscriptionController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using Filters.WebApi;
    using Logic;
    using Logic.Authentication;
    using Models;
    using PartnerCenter.Models;
    using PartnerCenter.Models.Customers;
    using PartnerCenter.Models.Subscriptions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    /// <summary>
    /// Provides the ability to manage subscriptions.
    /// </summary>
    [RoutePrefix("api/subscription")]
    public class SubscriptionController : BaseApiController
    {
        /// <summary>
        /// Initialize a new instance of <see cref="SubscriptionController" /> class.
        /// </summary>
        /// <param name="service">Provides access to all of the core services.</param>
        /// <exception cref="ArgumentNullException">
        /// service
        /// </exception>
        public SubscriptionController(IExplorerService service)
            : base(service)
        { }

        [@Authorize(UserRole = UserRole.Any)]
        [HttpGet]
        [Route("summary")]
        public async Task<SubscriptionsSummaryViewModel> GetSubscriptionsSummaryAsync(string customerId = "")
        {
            Customer customer;
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMeasurements;
            Dictionary<string, string> eventProperties;
            List<SubscriptionViewModel> models;
            ResourceCollection<Subscription> subscriptions;

            try
            {
                startTime = DateTime.Now;
                principal = (CustomerPrincipal)HttpContext.Current.User;

                if (string.IsNullOrEmpty(customerId) || !principal.IsAdmin)
                {
                    customerId = principal.CustomerId;
                }

                // Obtain a reference to the customer who owns the subscription(s).
                customer = await Services.PartnerCenter.Customers.ById(customerId).GetAsync();

                // Obtain a list of all subscriptions that the customer owns.
                subscriptions = await Services.PartnerCenter.Customers.ById(customerId)
                    .Subscriptions.GetAsync();

                models = subscriptions.Items.Select(subscription => new SubscriptionViewModel()
                {
                    CompanyName = customer.CompanyProfile.CompanyName,
                    CustomerId = customer.Id,
                    FriendlyName = subscription.FriendlyName,
                    Quantity = subscription.Quantity,
                    Status = subscription.Status,
                    SubscriptionId = subscription.Id,
                    UnitType = subscription.UnitType
                }).ToList();

                // Track the event measurements for analysis.
                eventMeasurements = new Dictionary<string, double>()
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "SubscriptionCount", models.Count }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>()
                {
                    { "CustomerId", customerId },
                    { "Email", principal.Email  },
                    { "IsAdmin", principal.IsAdmin.ToString() },
                    { "IsCustomer", principal.IsCustomer.ToString() },
                    { "PrincipalCustomerId", principal.CustomerId }
                };

                Services.Telemetry.TrackEvent("/api/subscription/summary", eventProperties, eventMeasurements);

                return new SubscriptionsSummaryViewModel()
                {
                    CustomerId = customerId,
                    Subscriptions = models
                };
            }
            finally
            {
                customer = null;
                eventMeasurements = null;
                eventProperties = null;
                models = null;
                principal = null;
                subscriptions = null;
            }
        }
    }
}