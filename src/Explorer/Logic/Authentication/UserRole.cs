// -----------------------------------------------------------------------
// <copyright file="UserRole.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Authentication
{
    /// <summary>
    /// Defines different user roles.
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// A customer or a partner.
        /// </summary>
        Any,
        /// <summary>
        /// A customer of the partner.
        /// </summary>
        Customer,
        /// <summary>
        /// An unauthenticated user.
        /// </summary>
        None,
        /// <summary>
        /// A partner user.
        /// </summary>
        Partner
    }
}