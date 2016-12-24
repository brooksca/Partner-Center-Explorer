// -----------------------------------------------------------------------
// <copyright file="CustomerControllerTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Tests.Controllers
{
    using Explorer.Controllers;
    using Logic;
    using QualityTools.Testing.Fakes;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Fakes;
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Provides unit tests for the <see cref="CustomerController"/> class.
    /// </summary>
    [TestClass]
    public class CustomerControllerTests
    {
        private readonly TestExplorerService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerControllerTests"/> class.
        /// </summary>
        public CustomerControllerTests()
        {
            _service = new TestExplorerService();
        }

        /// <summary>
        /// Validates that a <see cref="ArgumentNullException"/> is thrown by the constructor 
        /// when a null value is passed for the service parameter.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "service")]
        public void CustomerControllerVerifyCstrDomainException()
        {
            using (CustomerController c = new CustomerController(null))
            { }
        }

        /// <summary>
        /// Validates that an administrative user can retrieve a specific customer. 
        /// </summary>
        /// <returns>A task for asynchronous purposes.</returns>
        [TestMethod]
        public async Task CustomerControllerGetCustomerSummaryResultAsAdminAsync()
        {
            using (CustomerController c = new CustomerController(_service))
            {
                using (ShimsContext.Create())
                {
                    ShimHttpContext.CurrentGet = () => TestHelper.GetHttpContext(TestHelper.AccountId);

                    Models.CustomerSummaryViewModel model = await c.GetCustomerSummaryAsync(TestHelper.ContosoTenantId);

                    Assert.AreEqual(model.Id, TestHelper.ContosoTenantId);
                }
            }
        }

        /// <summary>
        /// Validates that a customer user can retrieve their customer object. 
        /// </summary>
        /// <returns>A task for asynchronous purposes.</returns>
        [TestMethod]
        public async Task CustomerControllerGetCustomerSummaryResultAsCustomerAsync()
        {
            using (CustomerController c = new CustomerController(_service))
            {
                using (ShimsContext.Create())
                {
                    ShimHttpContext.CurrentGet = () => TestHelper.GetHttpContext(TestHelper.ContosoTenantId);

                    Models.CustomerSummaryViewModel model = await c.GetCustomerSummaryAsync(TestHelper.ContosoTenantId);

                    Assert.AreEqual(model.Id, TestHelper.ContosoTenantId);
                }
            }
        }

        /// <summary>
        /// Validates that a customer user can will only be able to retrieve their customer object. 
        /// </summary>
        /// <returns>A task for asynchronous purposes.</returns>
        [TestMethod]
        public async Task CustomerControllerGetCustomerSummaryResultAsCustomerWithIncorrectIdAsync()
        {
            using (CustomerController c = new CustomerController(_service))
            {
                using (ShimsContext.Create())
                {
                    ShimHttpContext.CurrentGet = () => TestHelper.GetHttpContext(TestHelper.ContosoTenantId);

                    Models.CustomerSummaryViewModel model = await c.GetCustomerSummaryAsync(TestHelper.FabrikamTenantId);

                    // The GetCustomerSummaryAsync function should have overwritten the incorrect 
                    // customer identifier with the CustomerId value from the HttpContext. 
                    Assert.AreEqual(model.Id, TestHelper.ContosoTenantId);
                }
            }
        }

        /// <summary>
        /// Validates that an administrative user can retrieve all customers. 
        /// </summary>
        /// <returns>A task for asynchronous purposes.</returns>
        [TestMethod]
        public async Task CustomerControllerVerifyGetCustomersResultAsync()
        {
            using (CustomerController c = new CustomerController(_service))
            {
                using (ShimsContext.Create())
                {
                    ShimHttpContext.CurrentGet = () => TestHelper.GetHttpContext(TestHelper.AccountId);

                    Models.CustomersViewModel models = await c.GetCustomersAsync();
                    Assert.AreEqual(3, models.Customers.Count);
                }
            }
        }

        /// <summary>
        /// Validates that a customer user can only retrieve their customer. 
        /// </summary>
        /// <returns>A task for asynchronous purposes.</returns>
        [TestMethod]
        public async Task CustomerControllerVerifyGetCustomersResultAsCustomerAsync()
        {
            using (CustomerController c = new CustomerController(_service))
            {
                using (ShimsContext.Create())
                {
                    ShimHttpContext.CurrentGet = () => TestHelper.GetHttpContext(TestHelper.ContosoTenantId);

                    Models.CustomersViewModel models = await c.GetCustomersAsync();

                    Assert.AreEqual(1, models.Customers.Count);
                    Assert.AreEqual(TestHelper.ContosoTenantId, models.Customers.First().Id);
                }
            }
        }
    }
}