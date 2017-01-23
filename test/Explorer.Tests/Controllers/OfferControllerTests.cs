// -----------------------------------------------------------------------
// <copyright file="OfferControllerTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Explorer.Controllers;
    using Logic;
    using Models;
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Provides unit tests for the <see cref="OfferController"/> class.
    /// </summary>
    [TestClass]
    public class OfferControllerTests
    {
        private readonly TestExplorerService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferControllerTests"/> class.
        /// </summary>
        public OfferControllerTests()
        {
            _service = new TestExplorerService();
        }

        /// <summary>
        /// Validates that a <see cref="ArgumentNullException"/> is thrown by the constructor 
        /// when a null value is passed for the service parameter.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "service")]
        public void OfferControllerVerifyCstrDomainException()
        {
            using (OfferController c = new OfferController(null))
            { }
        }

        /// <summary>
        /// Validates that only valid offers are retrieved.
        /// </summary>
        /// <returns>A task for asynchronous purposes.</returns>
        [TestMethod]
        public async Task GetOffersValidationTestAsync()
        {
            IEnumerable<OfferViewModel> offers;

            try
            {
                using (OfferController c = new OfferController(_service))
                {
                    offers = await c.GetOffersAsync();
                    Assert.AreEqual(offers.Count(), 2);
                }
            }
            finally
            {
                offers = null;
            }
        }
    }
}