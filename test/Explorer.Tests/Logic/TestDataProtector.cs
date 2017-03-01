// -----------------------------------------------------------------------
// <copyright file="TestDataProtector.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Tests.Logic
{
    using System;
    using System.Text;
    using Security;

    /// <summary>
    /// Provides data protection exclusively for testing purposes.
    /// </summary>
    public class TestDataProtector : IDataProtector
    {
        /// <summary>
        /// Protects the specified data by encrypting.
        /// </summary>
        /// <param name="data">The data to be encrypted.</param>
        /// <returns>Base64 encoded string that represented the protected data.</returns>
        /// <exception cref="ArgumentException">
        /// data
        /// </exception>
        public string Protect(string data)
        {
            data.AssertNotEmpty(nameof(data));

            return Convert.ToBase64String(Encoding.ASCII.GetBytes(data));
        }

        /// <summary>
        /// Unprotects the specified data, which was protected by the <see cref="Protect(string)"/> method.
        /// </summary>
        /// <param name="data">The ciphertext data to unprotect.</param>
        /// <returns>The decrypted data in plaintext.</returns>
        /// <exception cref="System.ArgumentException">
        /// data
        /// </exception>
        public string Unprotect(string data)
        {
            data.AssertNotEmpty(nameof(data));
            return Encoding.ASCII.GetString(Convert.FromBase64String(data));
        }
    }
}