// -----------------------------------------------------------------------
// <copyright file="CustomerController.cs" company="Microsoft">
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
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    /// <summary>
    /// Provides the ability to manage customers.
    /// </summary>
    [RoutePrefix("api/customer")]
    public class CustomerController : BaseApiController
    {
        /// <summary>
        /// Initialize a new instance of <see cref="CustomerController" /> class.
        /// </summary>
        /// <param name="service">Provides access to all of the core services.</param>
        /// <exception cref="ArgumentNullException">
        /// service
        /// </exception>
        public CustomerController(IExplorerService service)
            : base(service)
        { }

        [@Authorize(UserRole = UserRole.Any)]
        [HttpGet]
        [Route("summary")]
        public async Task<CustomerSummaryViewModel> GetCustomerSummaryAsync(string customerId)
        {
            Customer customer;
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMeasurements;
            Dictionary<string, string> eventProperties;

            customerId.AssertNotEmpty(nameof(customerId));

            try
            {
                startTime = DateTime.Now;

                principal = (CustomerPrincipal)HttpContext.Current.User;

                if (principal.IsAdmin)
                {
                    customer = await Services.PartnerCenter.Customers.ById(customerId).GetAsync();
                }
                else
                {
                    // The authenticated users is not a partner administrator. This means the
                    // user is a global administrator for the customer their account is 
                    // associated with. In this situation only the customer the authenticated 
                    // user belongs to should be returned.
                    customer = await Services.PartnerCenter.Customers.ById(principal.CustomerId).GetAsync();
                }

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>()
                {
                    { "CustomerId", customerId },
                    { "Email", principal.Email  },
                    { "IsAdmin", principal.IsAdmin.ToString() },
                    { "IsCustomer", principal.IsCustomer.ToString() },
                    { "PrincipalCustomerId", principal.CustomerId }
                };

                // Track the event measurements for analysis.
                eventMeasurements = new Dictionary<string, double>()
                {
                    {"ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                Services.Telemetry.TrackEvent("/api/customer/summary", eventProperties, eventMeasurements);

                return new CustomerSummaryViewModel()
                {
                    BillingProfile = customer.BillingProfile,
                    CommerceId = customer.CommerceId,
                    CompanyProfile = customer.CompanyProfile,
                    Id = customer.Id
                };
            }
            finally
            {
                customer = null;
                eventMeasurements = null;
                eventProperties = null;
                principal = null;
            }
        }

        /// <summary>
        /// Retrieves a list of all customers.
        /// </summary>
        /// <returns>
        /// The list of customers utilized for rendering purpose.
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
            Customer customer;
            CustomerPrincipal principal;
            CustomersViewModel viewModel;
            DateTime startTime;
            Dictionary<string, double> eventMeasurements;
            Dictionary<string, string> eventProperties;
            SeekBasedResourceCollection<Customer> customers;

            try
            {
                startTime = DateTime.Now;

                principal = (CustomerPrincipal)HttpContext.Current.User;

                viewModel = new CustomersViewModel()
                {
                    Customers = new List<Customer>()
                };

                if (principal.IsAdmin)
                {
                    customers = await Services.PartnerCenter.Customers.GetAsync();
                    viewModel.Customers.AddRange(customers.Items);
                }
                else
                {
                    // The authenticated users is not a partner administrator. This means the
                    // user is a global administrator for the customer their account is 
                    // associated with. In this situation only the customer the authenticated 
                    // user belongs to should be returned.
                    customer = await Services.PartnerCenter.Customers.ById(principal.CustomerId).GetAsync();
                    viewModel.Customers.Add(customer);
                }

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>()
                {
                    { "Email", principal.Email  },
                    { "IsAdmin", principal.IsAdmin.ToString() },
                    { "IsCustomer", principal.IsCustomer.ToString() },
                    { "PrincipalCustomerId", principal.CustomerId }
                };

                // Track the event measurements for analysis.
                eventMeasurements = new Dictionary<string, double>()
                {
                    { "CustomerCount", viewModel.Customers.Count },
                    {"ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
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
                principal = null;
            }
        }
    }
}