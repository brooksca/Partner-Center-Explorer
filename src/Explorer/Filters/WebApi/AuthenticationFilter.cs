// -----------------------------------------------------------------------
// <copyright file="AuthenticationFilter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Filters.WebApi
{
    using Logic.Authentication;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http.Filters;

    /// <summary>
    /// Augments Web API authentication by replacing the principal with a more usable custom principal object.
    /// </summary>
    public class AuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        /// <summary>
        /// Authenticates a web API incoming request.
        /// </summary>
        /// <param name="context">The authentication context.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task.</returns>
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            context.Principal = new CustomerPrincipal(HttpContext.Current.User as System.Security.Claims.ClaimsPrincipal);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Challenges the caller.
        /// </summary>
        /// <param name="context">The authentication context.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task.</returns>
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}