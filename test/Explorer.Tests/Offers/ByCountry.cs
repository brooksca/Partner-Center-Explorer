// -----------------------------------------------------------------------
// <copyright file="ByCountry.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Tests.Offers
{
    using GenericOperations;

    /// <summary>
    /// Provides operations interfaces based on the specified country.
    /// </summary>
    /// <typeparam name="TOperations"> The type of the operations to return.</typeparam>
    public class ByCountry<TOperations> : ICountrySelector<TOperations>
    {
        private TOperations _operations;

        /// <summary>
        /// Initializes a new instance of the <see cref="ByCountry{TOperations}"/> class.
        /// </summary>
        /// <param name="operations">The type of the operations to return.</param>
        public ByCountry(TOperations operations)
        {
            _operations = operations;
        }

        /// <summary>
        /// Customizes operations based on the given country.
        /// </summary>
        /// <param name="country">The country to be used by the returned operations.</param>
        /// <returns>An operations interface customized for the provided country.</returns>
        TOperations ICountrySelector<TOperations>.ByCountry(string country)
        {
            return _operations;
        }
    }
}