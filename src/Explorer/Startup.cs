// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

[assembly: Microsoft.Owin.OwinStartup(typeof(Microsoft.Store.PartnerCenter.Explorer.Startup))]

namespace Microsoft.Store.PartnerCenter.Explorer
{
    using global::Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}