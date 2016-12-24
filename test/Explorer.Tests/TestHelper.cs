// -----------------------------------------------------------------------
// <copyright file="TestHelper.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Tests
{
    using Azure.ActiveDirectory.GraphClient;
    using Explorer.Logic.Authentication;
    using PartnerCenter.Models.Customers;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// Helper class that provides common objects used for unit testing.
    /// </summary>
    internal static class TestHelper
    {
        /// <summary>
        /// Gets the account identifier value from the calling assembly's configuration file.
        /// </summary>
        public static string AccountId => ConfigurationManager.AppSettings["AccountId"];

        /// <summary>
        /// Gets the tenant identifer for the Contoso customer from the calling assembly's configuration file.
        /// </summary>
        public static string ContosoTenantId => ConfigurationManager.AppSettings["ContosoTenantId"];

        /// <summary>
        /// Gets the tenant identifier for the Fabrikam customer from the calling assembly's configuration file.
        /// </summary>
        public static string FabrikamTenantId => ConfigurationManager.AppSettings["FabrikamTenantId"];

        /// <summary>
        /// Gets the tenant identifier for the Tailspin Toys customer from the calling assembly's configuration file.
        /// </summary>
        public static string TailspinToysTenantId => ConfigurationManager.AppSettings["TailspinToysTenantId"];

        /// <summary>
        /// Gets an instance of <see cref="IActiveDirectoryClient"/> used exclusively for unit tests.
        /// </summary>
        public static IActiveDirectoryClient ActiveDirectoryClient
        {
            get
            {
                return new Azure.ActiveDirectory.GraphClient.Fakes.StubIActiveDirectoryClient()
                {
                    UsersGet = () => GetUsers()
                };
            }
        }

        /// <summary>
        /// Gets an instance of <see cref="ITokenManagement"/> used exclusively for unit tests.
        /// </summary>
        public static ITokenManagement TokenMgmt
        {
            get
            {
                return new Explorer.Logic.Authentication.Fakes.StubITokenManagement()
                {
                    GetAppOnlyTokenAsyncStringStringString = (authority, key, resource) =>
                        Task.FromResult(new AuthenticationToken("access", DateTime.Now.AddMinutes(30)))
                };
            }
        }

        /// <summary>
        /// Generates the collection of test customers. 
        /// </summary>
        /// <returns>A collection of customers.</returns>
        /// <remarks>The collection will always return the following three customers: Contoso, Fabrikam, and Tailspin Toys</remarks>
        public static List<Customer> GetCustomers()
        {
            return new List<Customer>()
            {
                new Customer()
                {
                    BillingProfile = new CustomerBillingProfile()
                    { /* intentionally left empty */ },
                    CommerceId = ContosoTenantId,
                    CompanyProfile = new CustomerCompanyProfile()
                    {
                        CompanyName = "Contoso"
                    },
                    Id = ContosoTenantId
                },
                new Customer()
                {
                    BillingProfile = new CustomerBillingProfile()
                    { /* intentionally left empty */ },
                    CommerceId = FabrikamTenantId,
                    CompanyProfile = new CustomerCompanyProfile()
                    {
                        CompanyName = "Fabrikam"
                    },
                    Id = FabrikamTenantId
                },
                new Customer()
                {
                    BillingProfile = new CustomerBillingProfile()
                    { /* intentionally left empty */ },
                    CommerceId = TailspinToysTenantId,
                    CompanyProfile = new CustomerCompanyProfile()
                    {
                        CompanyName = "Tailspin Toys"
                    },
                    Id = TailspinToysTenantId
                }
            };
        }

        /// <summary>
        /// Gets a configured instance of <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="id">The identifier that controls whether or not this context has admini privileges.</param>
        /// <returns>An aptly configured instance of <see cref="HttpContext"/>.</returns>
        public static HttpContext GetHttpContext(string id)
        {
            id.AssertNotEmpty(nameof(id));

            return new HttpContext(new HttpRequest(null, "http://tempuri.org", null), new HttpResponse(null))
            {
                User = new CustomerPrincipal()
                {
                    CustomerId = id,
                    Email = "test@unit.com",
                    Name = "Unit Test",
                    TenantId = id
                }
            };
        }

        private static IUserCollection GetUsers()
        {
            return null;
        }
    }
}