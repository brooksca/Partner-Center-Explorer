// -----------------------------------------------------------------------
// <copyright file="TestCustomer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Tests.Customers
{
    using CustomerDirectoryRoles;
    using CustomerUsers;
    using ManagedServices;
    using Orders;
    using PartnerCenter.Customers;
    using PartnerCenter.Customers.Profiles;
    using PartnerCenter.Models;
    using PartnerCenter.Models.Customers;
    using PartnerCenter.Models.Invoices;
    using PartnerCenter.Models.Subscriptions;
    using PartnerCenter.Offers;
    using Qualification;
    using Relationships;
    using ServiceRequests;
    using SubscribedSkus;
    using Subscriptions;
    using Subscriptions.Fakes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Usage;

    /// <summary>
    /// Custom <see cref="Customer"/> class utilized exclusively for testing.
    /// </summary>
    public class TestCustomer : ICustomer
    {
        /// <summary>
        /// Initialize a new instance of <see cref="TestCustomer"/>.
        /// </summary>
        /// <param name="customerId">Identifier of the customer where operations should be performed.</param>
        /// <exception cref="ArgumentException">
        /// customerId
        /// </exception>
        public TestCustomer(string customerId)
        {
            customerId.AssertNotEmpty(nameof(customerId));

            Context = customerId;
        }

        /// <summary>
        /// Gets the component context object.
        /// </summary>
        public string Context
        { get; private set; }

        /// <summary>
        /// Obtains the directory role behavior for the customer.
        /// </summary>
        public IDirectoryRoleCollection DirectoryRoles
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Obtains the managed services behavior for the customer.
        /// </summary>
        public IManagedServiceCollection ManagedServices
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Obtains the offer categories behavior for the customer.
        /// </summary>
        public ICustomerOfferCategoryCollection OfferCategories
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Obtains the offers behavior for the customer.
        /// </summary>
        public ICustomerOfferCollection Offers
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Obtains the orders behavior for the customer.
        /// </summary>
        public IOrderCollection Orders
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Obtains the partner behavior for the customer.
        /// </summary>
        public IPartner Partner
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Obtains the profiles behavior for the customer.
        /// </summary>
        public ICustomerProfileCollection Profiles
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Obtains the qualifications behavior for the customer.
        /// </summary>
        public ICustomerQualification Qualification
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Obtains the relationships behavior for the customer.
        /// </summary>
        public ICustomerRelationshipCollection Relationships
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Obtains the service requests behavior for the customer.
        /// </summary>
        public IServiceRequestCollection ServiceRequests
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Obtains the subscribed SKUs behavior for the customer.
        /// </summary>
        public ICustomerSubscribedSkuCollection SubscribedSkus
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Obtains the subscriptions behavior for the customer.
        /// </summary>
        public ISubscriptionCollection Subscriptions
        {
            get
            {
                return new StubISubscriptionCollection()
                {
                    GetAsync = () =>
                    {
                        return Task.FromResult(GetSubscriptions(5));
                    }
                };
            }
        }

        /// <summary>
        /// Obtains the usage budget behavior for the customer.
        /// </summary>
        public ICustomerUsageSpendingBudget UsageBudget
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Obtains the usage summary behavior for the customer.
        /// </summary>
        public ICustomerUsageSummary UsageSummary
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Obtains the users behavior for the customer.
        /// </summary>
        public ICustomerUserCollection Users
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Deletes the customer from a testing in production account. This 
        /// will not work for customers outside of the integration sandbox.
        /// </summary>
        public void Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the customer from a testing in production account. This 
        /// will not work for customers outside of the integration sandbox.
        /// </summary>
        public Task DeleteAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the customer information.
        /// </summary>
        /// <returns>The customer information.</returns>
        public Customer Get()
        {
            return TestHelper.GetCustomers().Single(x => x.Id.Equals(Context, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Asynchronously retrieves the customer information.
        /// </summary>
        /// <returns>The customer information.</returns>
        public Task<Customer> GetAsync()
        {
            return Task.FromResult(TestHelper.GetCustomers()
                .Single(x => x.Id.Equals(Context, StringComparison.CurrentCultureIgnoreCase)));
        }

        /// <summary>
        /// Generates a set number of subscriptions exclusively for unit testing.
        /// </summary>
        /// <param name="quantity">Number of subscriptions to be generated.</param>
        /// <returns>A collection of subscriptions.</returns>
        private static ResourceCollection<Subscription> GetSubscriptions(int quantity)
        {
            List<Subscription> subscriptions = new List<Subscription>();

            for (int i = 0; i < quantity; i++)
            {
                subscriptions.Add(new Subscription()
                {
                    AutoRenewEnabled = true,
                    BillingType = BillingType.License,
                    CommitmentEndDate = DateTime.Now.AddYears(1),
                    CreationDate = DateTime.Now.AddMonths(-1),
                    EffectiveStartDate = DateTime.Now.AddMonths(-1),
                    FriendlyName = $"Unit Test Subscription {i}",
                    Id = Guid.NewGuid().ToString(),
                    Quantity = i,
                    Status = SubscriptionStatus.Active,
                    UnitType = "Seats"
                });
            }

            return new ResourceCollection<Subscription>(subscriptions);
        }
    }
}