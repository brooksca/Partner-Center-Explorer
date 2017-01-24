// -----------------------------------------------------------------------
// <copyright file="ExplorerService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using System;
    using System.Threading.Tasks;
    using Authentication;
    using Cache;
    using Configuration;
    using Telemetry;

    /// <summary>
    /// Provides access to the core services.
    /// </summary>
    public class ExplorerService : IExplorerService
    {
        /// <summary>
        /// A flag indicating whether or not the service has been initialized.
        /// </summary>
        private static bool initialized;

        /// <summary>
        /// Provides the ability to cache often used objects. 
        /// </summary>
        private static ICacheService cache;

        /// <summary>
        /// Provides the ability to track telemetry data.
        /// </summary>
        private static ITelemetryProvider telemetry;

        /// <summary>
        /// Provides localization functionality for the portal.
        /// </summary>
        private static Localization localization;

        /// <summary>
        /// Gets the service that provides caching functionality.
        /// </summary>
        public ICacheService CachingService
        {
            get
            {
                if (cache != null)
                {
                    return cache;
                }

                cache = new CacheService();

                return cache;
            }
        }

        /// <summary>
        /// Gets the Partner Center service reference.
        /// </summary>
        public IAggregatePartner PartnerCenter => this.GetPartnerCenterClient().Invoke();

        /// <summary>
        /// Gets the telemetry service reference.
        /// </summary>
        public ITelemetryProvider Telemetry
        {
            get
            {
                if (telemetry != null)
                {
                    return telemetry;
                }

                if (string.IsNullOrEmpty(ApplicationConfiguration.AppInsightsInstrumentationKey))
                {
                    telemetry = new EmptyTelemetryProvider();
                }
                else
                {
                    telemetry = new ApplicationInsightsTelemetryProvider();
                }

                return telemetry;
            }
        }

        /// <summary>
        /// Initializes the application core services.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operations.</returns>
        public async Task InitializeAsync()
        {
            if (initialized && localization != null)
            {
                return;
            }

            localization = new Localization(this);

            // Initialize the localization service by invoking the InitializeAsync function.
            await localization.InitializeAsync();

            initialized = true;
        }

        /// <summary>
        /// Gets the localization functionality.
        /// </summary>
        /// <returns>An instance of <see cref="ILocalization"/></returns>
        /// <exception cref="LocalizationException">The explorer service has not been initialized.</exception>
        public ILocalization Localization()
        {
            if (initialized && localization != null)
            {
                return localization;
            }

            throw new LocalizationException(Resources.ExplorerServiceNotInitializedException);
        }

        /// <summary>
        /// Obtains a reference to <see cref="IAggregatePartner"/>.
        /// </summary>
        /// <returns>A delegate used to obtain an instance of <see cref="IAggregatePartner"/>.</returns>
        private Func<IAggregatePartner> GetPartnerCenterClient()
        {
            ITokenManagement tokenMgmt = new TokenManagement(this);

            return () => PartnerService.Instance.CreatePartnerOperations(
               tokenMgmt.GetPartnerCenterAppOnlyCredentials(
               $"{ApplicationConfiguration.ActiveDirectoryEndpoint}/{ApplicationConfiguration.PartnerCenterApplicationTenantId}"));
        }

        /// <summary>
        /// Obtains a reference to <see cref="IAggregatePartner"/>.
        /// </summary>
        /// <returns>A delegate used to obtain an instance of <see cref="IAggregatePartner"/>.</returns>
        private Func<IAggregatePartner> GetPartnerCenterUserClient()
        {
            ITokenManagement tokenMgmt = new TokenManagement(this);

            return () => PartnerService.Instance.CreatePartnerOperations(
               tokenMgmt.GetPartnerCenterAppPlusUserCredentials(
               $"{ApplicationConfiguration.ActiveDirectoryEndpoint}/{ApplicationConfiguration.PartnerCenterApplicationTenantId}"));
        }
    }
}