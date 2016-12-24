// -----------------------------------------------------------------------
// <copyright file="IExplorerService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using Cache;
    using System.Threading.Tasks;
    using Telemetry;

    /// <summary>
    /// Represents the core service that powers this application.
    /// </summary>
    public interface IExplorerService
    {
        /// <summary>
        /// Service that provides caching functionality.
        /// </summary>
        ICacheService CachingService
        { get; }

        /// <summary>
        /// Provides localization functionality.
        /// </summary>
        ILocalization Localization
        { get; }

        /// <summary>
        /// Provides the ability to interact with the Partner Center managed API.
        /// </summary>
        IAggregatePartner PartnerCenter
        { get; }

        /// <summary>
        /// Provides a mechanism to record telemetry data.
        /// </summary>
        ITelemetryProvider Telemetry
        { get; }

        /// <summary>
        /// Initializes the Explorer service and all the dependent services.
        /// </summary>
        /// <returns></returns>
        Task InitializeAsync();
    }
}