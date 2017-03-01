// -----------------------------------------------------------------------
// <copyright file="OrderController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using Filters.WebApi;
    using Logic;
    using Models;
    using PartnerCenter.Models.Customers;
    using PartnerCenter.Models.Orders;
    using RequestContext;
    using Security;

    /// <summary>
    /// Provides the ability to manage orders.
    /// </summary>
    [RoutePrefix("api/order")]
    public class OrderController : BaseApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderController"/> class.
        /// </summary>
        /// <param name="service">Provides access to core application services.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public OrderController(IExplorerService service) : base(service)
        {
        }

        /// <summary>
        /// Processes a new order to create the specified resources.
        /// </summary>
        /// <param name="orderViewModel">Information pertaining to the order.</param>
        /// <returns>A summary for all subscriptions that were created.</returns>
        [@Authorize(UserRole = UserRole.None)]
        [HttpPost]
        [Route("create")]
        public async Task<IEnumerable<SubscriptionSummaryViewModel>> CreateAsync([FromBody]OrderViewModel orderViewModel)
        {
            Customer customer;
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMeasurements;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IEnumerable<SubscriptionSummaryViewModel> viewModels;
            IPartner operations;
            Order newOrder;
            int lineItemNumber = 0;

            orderViewModel.AssertNotNull(nameof(orderViewModel));

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();

                operations =
                    Services.PartnerCenter.With(RequestContextFactory.Instance.Create(correlationId));

                principal = (CustomerPrincipal)HttpContext.Current.User;

                customer = await operations.Customers.ById(orderViewModel.CustomerId).GetAsync();

                newOrder = new Order
                {
                    LineItems = orderViewModel.Subscriptions.Select(s => new OrderLineItem
                    {
                        LineItemNumber = lineItemNumber++,
                        OfferId = s.OfferId,
                        Quantity = s.Quantity
                    })
                };

                newOrder = await operations.Customers.ById(orderViewModel.CustomerId).Orders.CreateAsync(newOrder);

                viewModels = newOrder.LineItems.Select(s => new SubscriptionSummaryViewModel
                {
                    CompanyName = customer.CompanyProfile.CompanyName,
                    CustomerId = customer.Id,
                    FriendlyName = s.FriendlyName,
                    SubscriptionId = s.SubscriptionId
                });

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
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfSubscriptionsCreated", viewModels.Count() }
                };

                Services.Telemetry.TrackEvent("/api/order/create", eventProperties, eventMeasurements);

                return viewModels;
            }
            finally
            {
                eventMeasurements = null;
                eventProperties = null;
                newOrder = null;
                operations = null;
                principal = null;
            }
        }
    }
}