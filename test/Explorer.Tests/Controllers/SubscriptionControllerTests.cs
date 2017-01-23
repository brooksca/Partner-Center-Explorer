// -----------------------------------------------------------------------
// <copyright file="SubscriptionControllerTests.cs" company="Microsoft">
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
    /// Provides unit tests for the <see cref="SubscriptionController"/> class.
    /// </summary>
    [TestClass]
    public class SubscriptionControllerTests
    {
        private readonly TestExplorerService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionControllerTests"/> class.
        /// </summary>
        public SubscriptionControllerTests()
        {
            _service = new TestExplorerService();
        }

        /// <summary>
        /// Validates that a <see cref="ArgumentNullException"/> is thrown by the constructor 
        /// when a null value is passed for the service parameter.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SubscriptionControllerVerifyCstrDomainException()
        {
            using (SubscriptionController c = new SubscriptionController(null))
            { }
        }

        ///// <summary>
        ///// Validates that an administrative user can retrieved a sepcific customer summary.
        ///// </summary>
        ///// <returns>A task for asynchronous purposes.</returns>
        //[TestMethod]
        //public async Task SubscriptionControllerVerifyGetSubscriptionsSummaryAsync()
        //{
        //    using (SubscriptionController c = new SubscriptionController(_service))
        //    {
        //        using (ShimsContext.Create())
        //        {
        //            ShimHttpContext.CurrentGet = () => TestHelper.GetHttpContext(TestHelper.AccountId);

        //            Models.SubscriptionSummaryViewModel model = await c.GetSubscriptionsSummaryAsync(TestHelper.FabrikamTenantId);

        //            Assert.AreEqual(5, model.Subscriptions.Count());
        //        }
        //    }
        //}

        ///// <summary>
        ///// Validates that a customer user can only retrieve their own subscriptions.
        ///// </summary>
        ///// <returns>A task for asynchronous purposes.</returns>
        //[TestMethod]
        //public async Task SubscriptionControllerVerifyGetSubscriptionsSummaryAsCustomerAsync()
        //{
        //    // TODO - Add the necessary logic to verify that the subscriptions returned belong to the appropriate customer.

        //    using (SubscriptionController c = new SubscriptionController(_service))
        //    {
        //        using (ShimsContext.Create())
        //        {
        //            ShimHttpContext.CurrentGet = () => TestHelper.GetHttpContext(TestHelper.ContosoTenantId);

        //            Models.SubscriptionSummaryViewModel model = await c.GetSubscriptionsSummaryAsync(TestHelper.FabrikamTenantId);

        //            Assert.AreEqual(5, model.Subscriptions.Count());
        //        }
        //    }
        //}
    }
}