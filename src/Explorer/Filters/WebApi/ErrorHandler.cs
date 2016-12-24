// -----------------------------------------------------------------------
// <copyright file="ErrorHandler.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Filters.WebApi
{
    using System.Web.Http.Filters;

    public class ErrorHandler : ExceptionFilterAttribute
    {
        /// <summary>
        /// Intercepts unhandled exceptions and crafts the error response appropriately.
        /// </summary>
        /// <param name="actionExecutedContext">The context for the action.</param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnException(actionExecutedContext);
        }
    }
}