// -----------------------------------------------------------------------
// <copyright file="HomeControllerTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Tests.Controllers
{
    using Explorer.Controllers;
    using Logic;
    using System;
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Provides unit tests for the <see cref="HomeController"/> class.
    /// </summary>
    [TestClass]
    public class HomeControllerTests
    {
        private readonly TestExplorerService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeControllerTests"/> class.
        /// </summary>
        public HomeControllerTests()
        {
            _service = new TestExplorerService();
        }

        /// <summary>
        /// Validates that a <see cref="ArgumentNullException"/> is thrown by the constructor 
        /// when a null value is passed for the service parameter.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "service")]
        public void HomeControllerVerifyCstrDomainException()
        {
            using (HomeController c = new HomeController(null))
            { }
        }
    }
}