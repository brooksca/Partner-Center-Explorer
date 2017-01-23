// -----------------------------------------------------------------------
// <copyright file="IExplorerService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using System.Threading.Tasks;
    using Cache;
    using Telemetry;

    /// <summary>
    /// Represents the core service that powers this application.
    /// </summary>
    public interface IExplorerService
    {
        /// <summary>
        /// Gets the service that provides caching functionality.
        /// </summary>
        ICacheService CachingService { get; }

        /// <summary>
        /// Gets the localization functionality.
        /// </summary>
        ILocalization Localization { get; }

        /// <summary>
        /// Gets the Partner Center service reference.
        /// </summary>
        IAggregatePartner PartnerCenter { get; }

        /// <summary>
        /// Gets the telemetry service reference.
        /// </summary>
        ITelemetryProvider Telemetry { get; }

        /// <summary>
        /// Initializes the Explorer service and all the dependent services.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task InitializeAsync();
    }
}