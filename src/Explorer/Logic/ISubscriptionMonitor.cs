// -----------------------------------------------------------------------
// <copyright file="ISubscriptionMonitor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    /// <summary>
    /// Represents a method to obtain health information for a subscription.
    /// </summary>
    public interface ISubscriptionMonitor
    {
        /// <summary>
        /// Gets a list of active incidents for a subscription.
        /// </summary>
        /// <returns>A list of active incidents for a subscription.</returns>
        Task<List<IncidentModel>> GetIncidentsAsync();
    }
}