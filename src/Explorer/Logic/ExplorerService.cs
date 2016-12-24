// -----------------------------------------------------------------------
// <copyright file="ExplorerService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using Authentication;
    using Cache;
    using Configuration;
    using System;
    using System.Threading.Tasks;
    using Telemetry;

    /// <summary>
    /// Provides access to the core services.
    /// </summary>
    public class ExplorerService : IExplorerService
    {
        private static bool _initialized;
        private static ICacheService _cache;
        private static ITelemetryProvider _telemetry;
        private static Localization _localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplorerService"/> class.
        /// </summary>
        public ExplorerService()
        { }

        /// <summary>
        /// Service that provides caching functionality.
        /// </summary>
        public ICacheService CachingService
        {
            get
            {
                if (_cache != null)
                {
                    return _cache;
                }

                _cache = new CacheService();

                return _cache;
            }
        }

        /// <summary>
        /// Service provides localization functionality.
        /// </summary>
        public ILocalization Localization
        {
            get
            {
                if (_initialized && _localization != null)
                {
                    return _localization;
                }

                throw new LocalizationException(Resources.ExplorerServiceNotInitializedException);
            }
        }

        /// <summary>
        /// Service that interacts with the Partner Center Managed API.
        /// </summary>
        public IAggregatePartner PartnerCenter => GetPartnerCenterClient().Invoke();

        /// <summary>
        /// Service that provides a mechanism to record telemetry data.
        /// </summary>
        public ITelemetryProvider Telemetry
        {
            get
            {
                if (_telemetry != null)
                {
                    return _telemetry;
                }

                if (string.IsNullOrEmpty(ApplicationConfiguration.AppInsightsInstrumentationKey))
                {
                    _telemetry = new EmptyTelemetryProvider();
                }
                else
                {
                    _telemetry = new ApplicationInsightsTelemetryProvider(
                        ApplicationConfiguration.AppInsightsInstrumentationKey);
                }

                return _telemetry;
            }
        }

        /// <summary>
        /// Initializes the application core services.
        /// </summary>
        /// <returns>A task for asynchronous purposes.</returns>
        public async Task InitializeAsync()
        {
            if (_initialized && _localization != null)
            {
                return;
            }

            _localization = new Localization(this);

            // Initialize the localization service by invoking the InitializeAsync function.
            await _localization.InitializeAsync();

            _initialized = true;
        }

        /// <summary>
        /// Obtains a reference to <see cref="IAggregatePartner"/>.
        /// </summary>
        /// <returns>A delegate used to obtain an instance of <see cref="IAggregatePartner"/>.</returns>
        /// <remarks>
        /// This function returns a delegate beacuse a custom caching strategy it being utilized. Out of the box 
        /// the Partner Center SDK will leverage an in-memory technique for caching access tokens.
        /// </remarks>
        private Func<IAggregatePartner> GetPartnerCenterClient()
        {
            ITokenManagement tokenMgmt = new TokenManagement(this);

            return () => PartnerService.Instance.CreatePartnerOperations(
               tokenMgmt.GetPartnerCenterAppOnlyCredentials(
               $"{ApplicationConfiguration.ActiveDirectoryEndpoint}/{ApplicationConfiguration.AccountId}"));
        }
    }
}