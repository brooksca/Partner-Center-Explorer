// -----------------------------------------------------------------------
// <copyright file="Localization.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using PartnerCenter.Models.CountryValidationRules;
    using PartnerCenter.Models.Partners;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    public class Localization : ILocalization
    {
        private readonly IExplorerService _service;

        /// <summary>
        /// Initialize a new instance of the <see cref="Localization"/> class.
        /// </summary>
        /// <param name="service">Provides access to various services</param>
        /// <exception cref="System.ArgumentNullException">
        /// service
        /// </exception>
        public Localization(IExplorerService service)
        {
            service.AssertNotNull(nameof(service));

            _service = service;
        }

        /// <summary>
        /// Gets the country ISO2 code.
        /// </summary>
        public string CountryIso2Code
        { get; private set; }

        /// <summary>
        /// Gets the locale for the application.
        /// </summary>
        public string Locale
        { get; private set; }

        public async Task InitializeAsync()
        {
            CountryValidationRules countryValidationRules;
            LegalBusinessProfile businessProfile;
            RegionInfo regionInfo;

            try
            {
                // Obtain the business profile for the configured partner.
                businessProfile = await _service.PartnerCenter.Profiles.LegalBusinessProfile.GetAsync();
                // Initialize a new instance of the RegionInfo class. This will be used to obtain the 
                // approriate regional information for the configured partner.
                regionInfo = new RegionInfo(businessProfile.Address.Country);

                CountryIso2Code = businessProfile.Address.Country;

                // Obtain the country validation rules for the configured partner.
                countryValidationRules =
                     await _service.PartnerCenter.CountryValidationRules.ByCountry(CountryIso2Code).GetAsync();

                Locale = countryValidationRules.SupportedCulturesList.FirstOrDefault();
            }
            finally
            {
                businessProfile = null;
                countryValidationRules = null;
                regionInfo = null;
            }
        }
    }
}