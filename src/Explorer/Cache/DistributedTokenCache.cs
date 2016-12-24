// -----------------------------------------------------------------------
// <copyright file="DistributedTokenCache.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Cache
{
    using IdentityModel.Clients.ActiveDirectory;
    using Logic;
    using Practices.Unity;
    using System;
    using System.Linq;
    using System.Security.Claims;

    /// <summary>
    /// Custom implementation of the <see cref="TokenCache"/> class.
    /// </summary>
    /// <seealso cref="Microsoft.IdentityModel.Clients.ActiveDirectory.TokenCache" />
    public class DistributedTokenCache : TokenCache
    {
        private IExplorerService _service;
        private readonly string _key;
        private readonly string _resource;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedTokenCache"/> class.
        /// </summary>
        /// <param name="resource">The resource being accessed.</param>
        /// <exception cref="ArgumentException">
        /// resource
        /// </exception>
        public DistributedTokenCache(string resource)
        {
            resource.AssertNotEmpty(nameof(resource));

            _service = MvcApplication.UnityContainer.Resolve<IExplorerService>();
            _key = string.Empty;
            _resource = resource;

            AfterAccess = AfterAccessNotification;
            BeforeAccess = BeforeAccessNotification;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedTokenCache"/> class.
        /// </summary>
        /// <param name="resource">The resource being accessed.</param>
        /// <param name="key">Key used to access and store the access token.</param>
        /// <exception cref="ArgumentException">
        /// resource
        /// </exception>
        public DistributedTokenCache(string resource, string key)
        {
            key.AssertNotEmpty(nameof(key));
            resource.AssertNotEmpty(nameof(resource));

            _service = MvcApplication.UnityContainer.Resolve<IExplorerService>();
            _key = key;
            _resource = resource;

            AfterAccess = AfterAccessNotification;
            BeforeAccess = BeforeAccessNotification;
        }

        /// <summary>
        /// Notification method called after any library method accesses the cache.
        /// </summary>
        /// <param name="args">Contains parameters used by the ADAL call accessing the cache.</param>
        public void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            if (!HasStateChanged)
            {
                return;
            }

            if (Count > 0)
            {
                _service.CachingService.Store(
                    CacheDatabaseType.Authentication, Key, Convert.ToBase64String(Serialize()));
            }
            else
            {
                _service.CachingService.Delete(CacheDatabaseType.Authentication, Key);
            }

            HasStateChanged = false;
        }

        /// <summary>
        /// Notification method called before any library method accesses the cache.
        /// </summary>
        /// <param name="args">Contains parameters used by the ADAL call accessing the cache.</param>
        public void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            string value = _service.CachingService.Fetch<string>(CacheDatabaseType.Authentication, Key);

            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            Deserialize(Convert.FromBase64String(value));
        }

        /// <summary>
        /// Clears the cache by deleting all the items. Note that if the cache is the default shared cache, clearing it would
        /// impact all the instances of <see cref="T:Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext" /> which share that cache.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            _service.CachingService.Clear(CacheDatabaseType.Authentication);
        }

        /// <summary>
        /// Deletes an item from the cache.
        /// </summary>
        /// <param name="item">The item to delete from the cache.</param>
        public override void DeleteItem(TokenCacheItem item)
        {
            base.DeleteItem(item);
            _service.CachingService.Delete(CacheDatabaseType.Authentication, Key);
        }

        private string Key => string.IsNullOrEmpty(_key) ?
            $"Resource:{_resource}::Key:{ClaimsPrincipal.Current.Identities.First().FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value}" : $"Resource:{_resource}::Key:{_key}";
    }
}