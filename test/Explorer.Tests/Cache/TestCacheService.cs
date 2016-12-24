// -----------------------------------------------------------------------
// <copyright file="TestCacheService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Tests.Cache
{
    using Explorer.Cache;
    using Explorer.Logic;
    using Logic;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Caching;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides in-memory caching utilized exclusively for unit testing.
    /// </summary>
    public class TestCacheService : ICacheService
    {
        private readonly IDataProtector _protector;
        private static MemoryCache _cache;
        private static object _lock = new object();

        /// <summary>
        /// Initialize a new instance of the <see cref="TestCacheService"/> class.
        /// </summary>
        public TestCacheService()
        {
            _protector = new TestDataProtector();
        }

        /// <summary>
        /// Removes all entities from the specified cache database. 
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        public async Task ClearAsync(CacheDatabaseType database)
        {
            lock (_lock)
            {
                foreach (KeyValuePair<string, object> entity in Cache)
                {
                    Cache.Remove(entity.Key);
                }
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Removes the specified entity from the cache.
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        /// <param name="key">A unique indentifier for the cache entry.</param>
        /// <exception cref="ArgumentException">
        /// key
        /// </exception>
        public async Task DeleteAsync(CacheDatabaseType database, string key)
        {
            key.AssertNotEmpty(nameof(key));

            lock (_lock)
            {
                if (Cache.Contains(key))
                {
                    Cache.Remove(key);
                }
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Fetches the specified entity from the cache.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity in cache.</typeparam>
        /// <param name="database">Cache database type where the data is stored.</param>
        /// <param name="key">A unique indentifier for the cache entry.</param>
        /// <returns>
        /// The entity associated with the specified key.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// key
        /// </exception>
        public TEntity Fetch<TEntity>(CacheDatabaseType database, string key) where TEntity : class
        {
            key.AssertNotEmpty(nameof(key));

            lock (_lock)
            {
                if (!Cache.Contains(key))
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<TEntity>(_protector.Unprotect((string)Cache.Get(key)));
            }
        }

        /// <summary>
        /// Fetches the specified entity from the cache.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity in cache.</typeparam>
        /// <param name="database">Cache database type where the data is stored.</param>
        /// <param name="key">A unique indentifier for the cache entry.</param>
        /// <returns>
        /// The entity associated with the specified key.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// key
        /// </exception>
        public async Task<TEntity> FetchAsync<TEntity>(CacheDatabaseType database, string key) where TEntity : class
        {
            TEntity value;

            key.AssertNotEmpty(nameof(key));

            lock (_lock)
            {
                if (!Cache.Contains(key))
                {
                    return null;
                }

                value = JsonConvert.DeserializeObject<TEntity>(_protector.Unprotect((string)Cache.Get(key)));
            }

            return await Task.FromResult(value);
        }

        /// <summary>
        /// Stores the specified entity in the cache.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity in cache.</typeparam>
        /// <param name="database">Cache database type where the data should be stored.</param>
        /// <param name="key">A unique indentifier for the cache entry.</param>
        /// <param name="entity">The object to be cahced.</param>
        /// <param name="expiration">When the cached object expires.</param>
        /// <exception cref="ArgumentException">
        /// key 
        /// </exception>
        /// <exception cref="ArgumentNullException"> 
        /// entity
        /// </exception>
        public async Task StoreAsync<TEntity>(CacheDatabaseType database, string key, TEntity entity, TimeSpan? expiration = default(TimeSpan?)) where TEntity : class
        {
            key.AssertNotEmpty(nameof(key));
            entity.AssertNotNull(nameof(entity));

            lock (_lock)
            {
                if (expiration.HasValue)
                {
                    Cache.Add(
                        key,
                        _protector.Protect(JsonConvert.SerializeObject(entity)),
                        new CacheItemPolicy() { AbsoluteExpiration = DateTime.Now.Add(expiration.Value) });
                }
                else
                {
                    Cache.Add(
                        new CacheItem(key, _protector.Protect(JsonConvert.SerializeObject(entity))), new CacheItemPolicy());
                }
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Removes all entities from the specified cache databae. 
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        public void Clear(CacheDatabaseType database)
        {
            lock (_lock)
            {
                foreach (KeyValuePair<string, object> entity in Cache)
                {
                    Cache.Remove(entity.Key);
                }
            }
        }

        /// <summary>
        /// Removes the specified entity from the cache.
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        /// <param name="key">A unique indentifier for the cache entry.</param>
        /// <exception cref="ArgumentException">
        /// key
        /// </exception>
        public void Delete(CacheDatabaseType database, string key)
        {
            key.AssertNotEmpty(nameof(key));

            lock (_lock)
            {
                if (Cache.Contains(key))
                {
                    Cache.Remove(key);
                }
            }
        }

        /// <summary>
        /// Stores the specified entity in the cache.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity in cache.</typeparam>
        /// <param name="database">Cache database type where the data should be stored.</param>
        /// <param name="key">A unique indentifier for the cache entry.</param>
        /// <param name="entity">The object to be cahced.</param>
        /// <param name="expiration">When the cached object expires.</param>
        /// <exception cref="ArgumentException">
        /// key 
        /// </exception>
        /// <exception cref="ArgumentNullException"> 
        /// entity
        /// </exception>
        public void Store<TEntity>(CacheDatabaseType database, string key, TEntity entity, TimeSpan? expiration = default(TimeSpan?)) where TEntity : class
        {
            key.AssertNotEmpty(nameof(key));
            entity.AssertNotNull(nameof(entity));

            lock (_lock)
            {
                if (expiration.HasValue)
                {
                    Cache.Add(
                        key,
                        _protector.Protect(JsonConvert.SerializeObject(entity)),
                        new CacheItemPolicy() { AbsoluteExpiration = DateTime.Now.Add(expiration.Value) });
                }
                else
                {
                    Cache.Add(
                        new CacheItem(key, _protector.Protect(JsonConvert.SerializeObject(entity))), new CacheItemPolicy());
                }
            }
        }

        /// <summary>
        /// Gets a reference to the in-memory cache data structure.
        /// </summary>
        private MemoryCache Cache => _cache ?? (_cache = new MemoryCache(typeof(CacheService).FullName));
    }
}