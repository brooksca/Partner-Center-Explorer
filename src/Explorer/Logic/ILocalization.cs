// -----------------------------------------------------------------------
// <copyright file="ILocalization.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using System.Threading.Tasks;

    public interface ILocalization
    {
        /// <summary>
        /// Gets the country ISO2 code.
        /// </summary>
        string CountryIso2Code
        { get; }

        /// <summary>
        /// Gets the locale for the application.
        /// </summary>
        string Locale
        { get; }

        /// <summary>
        /// Initializes a localization service.
        /// </summary>
        /// <returns>A task for asynchronous purposes.</returns>
        Task InitializeAsync();
    }
}