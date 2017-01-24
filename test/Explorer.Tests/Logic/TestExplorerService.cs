// -----------------------------------------------------------------------
// <copyright file="TestExplorerService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Tests.Logic
{
    using Cache;
    using Customers;
    using Explorer.Cache;
    using Explorer.Logic;
    using Explorer.Logic.Fakes;
    using PartnerCenter.Customers.Fakes;
    using PartnerCenter.Models;
    using PartnerCenter.Models.Customers;
    using PartnerCenter.Models.Invoices;
    using PartnerCenter.Models.Offers;
    using PartnerCenter.Offers;
    using PartnerCenter.Offers.Fakes;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Telemetry;

    /// <summary>
    /// Provides access to all of the core services and is only utilized for testing purposes.
    /// </summary>
    public class TestExplorerService : IExplorerService
    {
        private static ICacheService _cache;
        private static IAggregatePartner _partnerCenter;
        private static ITelemetryProvider _telemetry;
        private static ILocalization _localization;

        /// <summary>
        /// Service that provides caching functionality.
        /// </summary>
        public ICacheService CachingService => _cache ?? (_cache = new TestCacheService());

        /// <summary>
        /// Provides the ability to interact with the Partner Center managed API.
        /// </summary>
        public IAggregatePartner PartnerCenter => _partnerCenter ?? (_partnerCenter = GetTestPartnerCenter());

        /// <summary>
        /// Provides a mechanism to record telemetry data.
        /// </summary>
        public ITelemetryProvider Telemetry => _telemetry ?? (_telemetry = new EmptyTelemetryProvider());

        /// <summary>
        /// Initializes the Explorer service and all the dependent services.
        /// </summary>
        /// <returns></returns>
        public Task InitializeAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Provides localization functionality.
        /// </summary>
        public ILocalization Localization()
        {
            if (_localization == null)
            {
                _localization = new StubILocalization()
                {
                    CountryIso2CodeGet = () => "US",
                    LocaleGet = () => ""
                };
            }

            return _localization;
        }

        /// <summary>
        /// Obtains an instance of <see cref="IAggregatePartner"/> utilized exclusively for testing.
        /// </summary>
        /// <returns></returns>
        private static IAggregatePartner GetTestPartnerCenter()
        {
            StubIOfferCollection offerCollection = new StubIOfferCollection()
            {
                GetAsync = () => Task.FromResult(GetTestOffers())
            };

            return new Fakes.StubIAggregatePartner()
            {
                CustomersGet = () => new StubICustomerCollection()
                {
                    ByIdString = (customerId) => new TestCustomer(customerId),
                    GetAsync = () => Task.FromResult(new SeekBasedResourceCollection<Customer>(TestHelper.GetCustomers()))
                },
                OffersGet = () => new Offers.ByCountry<IOfferCollection>(offerCollection)
            };
        }

        /// <summary>
        /// Generates a collection of <see cref="Offer"/>s for testing purposes.
        /// </summary>
        /// <returns></returns>
        private static ResourceCollection<Offer> GetTestOffers()
        {
            List<Offer> offers = new List<Offer>
            {
                new Offer()
                {
                    Billing = BillingType.License,
                    Country = "US",
                    Description = "Test license based offer",
                    Id = "b78d52a9-e492-47e7-99e2-c4d8f2d0d769",
                    IsAddOn = false,
                    IsAvailableForPurchase = true
                },
                new Offer()
                {
                    Billing = BillingType.License,
                    Country = "US",
                    Description = "Test license based addon offer",
                    Id = Guid.NewGuid().ToString(),
                    IsAddOn = true,
                    IsAvailableForPurchase = true
                },
                new Offer()
                {
                    Billing = BillingType.License,
                    Country = "US",
                    Description = "Test license based offer with prerequisites",
                    Id = Guid.NewGuid().ToString(),
                    IsAddOn = true,
                    IsAvailableForPurchase = true,
                    PrerequisiteOffers = new [] { "b78d52a9-e492-47e7-99e2-c4d8f2d0d769" }
                },
                new Offer()
                {
                    Billing = BillingType.License,
                    Country = "US",
                    Description = "Test usage based offer",
                    Id = Guid.NewGuid().ToString(),
                    IsAddOn = false,
                    IsAvailableForPurchase = true
                },
                new Offer()
                {
                    Billing = BillingType.License,
                    Country = "US",
                    Description = "Test unavailable for purchase usage based offer",
                    Id = Guid.NewGuid().ToString(),
                    IsAddOn = false,
                    IsAvailableForPurchase = false
                }
            };

            return new ResourceCollection<Offer>(offers);
        }
    }
}