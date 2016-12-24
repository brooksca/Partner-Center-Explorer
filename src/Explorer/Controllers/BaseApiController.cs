// -----------------------------------------------------------------------
// <copyright file="BaseApiController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using Logic;
    using Practices.Unity;
    using System.Web.Http;

    public abstract class BaseApiController : ApiController
    {
        private readonly IExplorerService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApiController"/> class.
        /// </summary>
        /// <param name="service">Provides access to all of the core services.</param>
        /// <exception cref="System.ArgumentNullException">
        /// service
        /// </exception>
        protected BaseApiController(IExplorerService service)
        {
            service.AssertNotNull(nameof(service));

            _service = service;
        }

        /// <summary>
        /// Provides access to all of the core services.
        /// </summary>
        protected IExplorerService Services => _service ?? MvcApplication.UnityContainer.Resolve<IExplorerService>();
    }
}