// -----------------------------------------------------------------------
// <copyright file="OfferController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Cache;
    using Filters.WebApi;
    using Logic;
    using Logic.Authentication;
    using Models;
    using PartnerCenter.Models;
    using PartnerCenter.Models.Offers;

    /// <summary>
    /// Provides the ability to manage Partner Center offers.
    /// </summary>
    [RoutePrefix("api/offer")]
    public class OfferController : BaseApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferController" /> class.
        /// </summary>
        /// <param name="service">Provides access to all the core services.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public OfferController(IExplorerService service)
            : base(service)
        {
        }

        /// <summary>
        /// Obtain a list of available offers from Partner Center.
        /// </summary>
        /// <returns>A list of available offers from Partner Center.</returns>
        [@Authorize(UserRole = UserRole.Any)]
        [HttpGet]
        [Route("")]
        public async Task<IEnumerable<OfferViewModel>> GetOffersAsync()
        {
            DateTime startTime;
            Dictionary<string, double> eventMeasurements;
            Dictionary<string, string> eventProperties;
            IEnumerable<Offer> eligibleOffers;
            ResourceCollection<Offer> offers;

            try
            {
                startTime = DateTime.Now;

                offers = await Services.CachingService.FetchAsync<ResourceCollection<Offer>>(
                    CacheDatabaseType.DataStructures, "MicrosoftOffers");

                if (offers == null)
                {
                    offers = await Services.PartnerCenter.Offers.ByCountry(
                        Services.Localization().CountryIso2Code).GetAsync();

                    await Services.CachingService.StoreAsync(
                        CacheDatabaseType.DataStructures, "MicrosoftOffers", offers, TimeSpan.FromDays(1));
                }

                eligibleOffers = offers?.Items.Where(offer =>
                        !offer.IsAddOn &&
                        (offer.PrerequisiteOffers == null || !offer.PrerequisiteOffers.Any()) && offer.IsAvailableForPurchase);

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CountryIso2Code", Services.Localization().CountryIso2Code },
                };

                // Track the event measurements for analysis.
                eventMeasurements = new Dictionary<string, double>()
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                Services.Telemetry.TrackEvent("/api/offer", eventProperties, eventMeasurements);

                return eligibleOffers.Select(o => new OfferViewModel
                {
                    Category = o.Category.Name,
                    Description = o.Description,
                    Id = o.Id,
                    Name = o.Name
                });
            }
            finally
            {
                eventMeasurements = null;
                eventProperties = null;
                offers = null;
            }
        }
    }
}