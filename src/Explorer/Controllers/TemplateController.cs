// -----------------------------------------------------------------------
// <copyright file="TemplateController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using Configuration;
    using Configuration.Manager;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    /// <summary>
    /// Serves HTML templates to the browser.
    /// </summary>
    public class TemplateController : Controller
    {
        /// <summary>
        /// Serves the HTML template for the customer page presenter.
        /// </summary>
        /// <returns>The HTML template for the customer page presenter.</returns>
        public ActionResult Customer()
        {
            return PartialView();
        }

        /// <summary>
        /// Serves the HTML template for the homepage presenter.
        /// </summary>
        /// <returns>The HTML template for the homepage presenter.</returns>
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Home()
        {
            return PartialView();
        }

        /// <summary>
        /// Serves the HTML templates for the framework controls and services.
        /// </summary>
        /// <returns>The HTML template for the framework controls and services.</returns>
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public async Task<ActionResult> FrameworkFragments()
        {
            WebPortalConfigurationManager builder = ApplicationConfiguration.WebPortalConfigurationManager;

            ViewBag.Templates = (await builder.AggregateNonStartupAssets()).Templates;

            return PartialView();
        }
    }
}