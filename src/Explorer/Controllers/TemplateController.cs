// -----------------------------------------------------------------------
// <copyright file="TemplateController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Configuration;
    using Configuration.Manager;
    using Filters.Mvc;
    using Security;

    /// <summary>
    /// Serves HTML templates to the browser.
    /// </summary>
    public class TemplateController : Controller
    {
        /// <summary>
        /// Serves the HTML template for the add subscriptions presenter.
        /// </summary>
        /// <returns>The HTML template for the add subscriptions presenter.</returns>
        [HttpGet]
        [@Authorize(UserRole = UserRole.Any)]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult AddSubscriptions()
        {
            return this.PartialView();
        }

        /// <summary>
        /// Serves the HTML template for the customer page presenter.
        /// </summary>
        /// <returns>The HTML template for the customer page presenter.</returns>
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Customer()
        {
            return this.PartialView();
        }

        /// <summary>
        /// Serves the HTML template for the customer registration presenter.
        /// </summary>
        /// <returns>The HTML template for the customer registration presenter.</returns>
        [HttpGet]
        [@Authorize(UserRole = UserRole.Partner)]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult CustomerAddNew()
        {
            return this.PartialView();
        }

        /// <summary>
        /// Serves the HTML template for the homepage presenter.
        /// </summary>
        /// <returns>The HTML template for the homepage presenter.</returns>
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Home()
        {
            return this.PartialView();
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

            return this.PartialView();
        }

        /// <summary>
        /// Serves the HTML template for the registration confirmation page.
        /// </summary>
        /// <returns>The HTML template for the registration confirmation page.</returns>
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult RegistrationConfirmation()
        {
            return this.PartialView(); 
        }

        /// <summary>
        /// Serves the HTML template for the subscription page presenter.
        /// </summary>
        /// <returns>The HTML template for the subscription page presenter.</returns>
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Subscription()
        {
            return this.PartialView();
        }
    }
}