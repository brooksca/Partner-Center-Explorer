// -----------------------------------------------------------------------
// <copyright file="TokenType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Authentication
{
    /// <summary>
    /// Defines the possible token types.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// The token was obtained using App Only authentication.
        /// </summary>
        AppOnly,

        /// <summary>
        /// The token was obtained using App + User authentication.
        /// </summary>
        AppPlusUser
    }
}