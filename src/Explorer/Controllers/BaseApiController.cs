// -----------------------------------------------------------------------
// <copyright file="BaseApiController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System.Web.Http;
    using Logic;

    /// <summary>
    /// Base controller for all API controllers.
    /// </summary>
    public abstract class BaseApiController : ApiController
    {
        /// <summary>
        /// Provides access to the core application services.
        /// </summary>
        private readonly IExplorerService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApiController"/> class.
        /// </summary>
        /// <param name="service">Provides access to core application services.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        protected BaseApiController(IExplorerService service)
        {
            service.AssertNotNull(nameof(service));

            this.service = service;
        }

        /// <summary>
        /// Provides access to the core application services.
        /// </summary>
        protected IExplorerService Services => this.service;
    }
}