// -----------------------------------------------------------------------
// <copyright file="ActiveDirectoryTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Tests.Logic.Azure
{
    using Explorer.Logic;
    using Explorer.Logic.Azure;
    using Logic;
    using System;
    using System.Threading.Tasks;
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Provides unit test for the <see cref="ActiveDirectory"/> class.
    /// </summary>
    [TestClass]
    public class ActiveDirectoryTests
    {
        private readonly IExplorerService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveDirectoryTests"/> class.
        /// </summary>
        public ActiveDirectoryTests()
        {
            _service = new TestExplorerService();
        }

        /// <summary>
        /// Verifies that an <see cref="ArgumentException"/> is thrown when a tenant identifier is not specified.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "tenantId")]
        public void ActiveDirectoryVerifyCstrTenantIdException()
        {
            ActiveDirectory ad = new ActiveDirectory(_service, string.Empty);
        }

        /// <summary>
        /// Verifies that an <see cref="ArgumentException"/> is thrown when an assertion token is not specified.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "assertionToken")]
        public void ActiveDirectoryVerifyCstrAssertionTokenException()
        {
            ActiveDirectory ad = new ActiveDirectory(_service, TestHelper.ContosoTenantId, string.Empty);
        }

        /// <summary>
        /// Verifies that an <see cref="ArgumentException"/> is thrown when a code is not specified.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "code")]
        public void ActiveDirectoryVerifyCstrCodeException()
        {
            ActiveDirectory ad = new ActiveDirectory(
                _service,
                TestHelper.ContosoTenantId,
                string.Empty,
                Guid.NewGuid().ToString(),
                new Uri("https://graph.windows.net"));
        }

        /// <summary>
        /// Verifies that an <see cref="ArgumentException"/> is thrown when a redirect URI is not specified.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "redirectUri")]
        public void ActiveDirectoryVerifyCstrRedirectUriException()
        {
            ActiveDirectory ad = new ActiveDirectory(
                _service,
                TestHelper.ContosoTenantId,
                "123456",
                Guid.NewGuid().ToString(),
                null);
        }

        /// <summary>
        /// Verifies that an <see cref="ArgumentException"/> is thrown when an object identifier is not specified.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "objectId")]
        public async Task GetDirectoryRolesObjectIdExceptionTestAsync()
        {
            ActiveDirectory ad = new ActiveDirectory(TestHelper.ActiveDirectoryClient, _service);

            await ad.GetDirectoryRolesAsync(string.Empty);
        }
    }
}