// -----------------------------------------------------------------------
// <copyright file="AiHandleErrorAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Filters.Mvc
{
    using Logic;
    using Practices.Unity;
    using System;
    using System.Web.Mvc;

    /// <summary>
    /// Attribute used to track exceptions using Application Insights.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AiHandleErrorAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// Called when an exception occurs.
        /// </summary>
        /// <param name="filterContext">The action-filter context.</param>
        /// <remarks>If customerError is set to Off then the exception will be reported through telemetry.</remarks>
        public override void OnException(ExceptionContext filterContext)
        {
            IExplorerService service = MvcApplication.UnityContainer.Resolve<IExplorerService>();

            if (filterContext != null && filterContext.HttpContext != null && filterContext.Exception != null)
            {
                if (filterContext.HttpContext.IsCustomErrorEnabled)
                {
                    service.Telemetry.TrackException(filterContext.Exception);
                }
            }

            base.OnException(filterContext);
        }
    }
}