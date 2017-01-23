// -----------------------------------------------------------------------
// <copyright file="IncidentModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    /// <summary>
    /// Model that describes a service incident.
    /// </summary>
    public class IncidentModel
    {
        /// <summary>
        /// Gets or sets the description of the incident.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the incident.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the incident.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the status for the incident.
        /// </summary>
        public string Status { get; set; }
    }
}