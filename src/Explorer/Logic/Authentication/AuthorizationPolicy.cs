// -----------------------------------------------------------------------
// <copyright file="AuthorizationPolicy.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Authentication
{
    /// <summary>
    /// Represents the authorization policy utilized by this application.
    /// </summary>
    public class AuthorizationPolicy
    {
        /// <summary>
        /// Gets a boolean value that indicates whether or not the user is authorized.
        /// </summary>
        /// <param name="principal">
        /// An instance of <see cref="CustomerPrincipal"/> that represents the authenticated user.
        /// </param>
        /// <param name="requiredUserRole">The role required for the operation being attempted.</param>
        /// <returns>
        ///     <c>true</c> if the user is authorized; otherwise <c>false</c>
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// principal 
        /// </exception>
        public bool IsAuthorized(CustomerPrincipal principal, UserRole requiredUserRole)
        {
            principal.AssertNotNull(nameof(principal));

            bool isAuthorized;

            switch (requiredUserRole)
            {
                case UserRole.Customer:
                    isAuthorized = principal.IsCustomer && !principal.IsAdmin;
                    break;
                case UserRole.Partner:
                    isAuthorized = !principal.IsCustomer && principal.IsAdmin;
                    break;
                case UserRole.Any:
                    isAuthorized = principal.IsCustomer || principal.IsAdmin;
                    break;
                case UserRole.None:
                    isAuthorized = !principal.IsCustomer && !principal.IsAdmin;
                    break;
                default:
                    isAuthorized = false;
                    break;
            }

            return isAuthorized;
        }
    }
}