// -----------------------------------------------------------------------
// <copyright file="IActiveDirectory.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Azure
{
    using Microsoft.Azure.ActiveDirectory.GraphClient;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an object that interacts with Azure Active Directory.
    /// </summary>
    public interface IActiveDirectory
    {
        /// <summary>
        /// Obtains a list of directory roles that the specified directory is associated with.
        /// </summary>
        /// <param name="objectId">Object identifier for the object to be checked.</param>
        /// <returns>A list of directory that the specified object identifier is associated with.</returns>
        /// <exception cref="System.ArgumentException">
        /// objectId
        /// </exception>
        Task<List<IDirectoryRole>> GetDirectoryRolesAsync(string objectId);

        /// <summary>
        /// Obtains a list service configuration records for all domains that are associated with the tenant.
        /// </summary>
        /// <returns>A list of service configuration records for all domains associated with the tenant.</returns>
        Task<List<IDomainDnsRecord>> GetDomainDnsRecordsAsync();
    }
}