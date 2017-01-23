// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using System;

    /// <summary>
    /// Provides useful methods used for validation.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Appends a details to a partner domain exception.
        /// </summary>
        /// <param name="exception">The exception to append to.</param>
        /// <param name="key">The detail key.</param>
        /// <param name="value">The detail value.</param>
        /// <returns>The updated exception.</returns>
        public static PartnerDomainException AddDetail(this PartnerDomainException exception, string key, string value)
        {
            exception.AssertNotNull(nameof(exception));
            key.AssertNotEmpty(nameof(key));
            value.AssertNotEmpty(nameof(value));

            exception.Details[key] = value;

            return exception;
        }

        /// <summary>
        /// Ensures that a string is not empty.
        /// </summary>
        /// <param name="nonEmptyString">The string to validate.</param>
        /// <param name="caption">The name to report in the exception.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="nonEmptyString"/> is empty or null.
        /// </exception>
        public static void AssertNotEmpty(this string nonEmptyString, string caption)
        {
            if (string.IsNullOrWhiteSpace(nonEmptyString))
            {
                throw new ArgumentException($"{caption ?? "string"} is not set");
            }
        }

        /// <summary>
        /// Ensures that a given object is not null. Throws an exception otherwise.
        /// </summary>
        /// <param name="objectToValidate">The object we are validating.</param>
        /// <param name="caption">The name to report in the exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="objectToValidate"/> is null.
        /// </exception>
        public static void AssertNotNull(this object objectToValidate, string caption)
        {
            if (objectToValidate == null)
            {
                throw new ArgumentNullException(caption);
            }
        }
    }
}