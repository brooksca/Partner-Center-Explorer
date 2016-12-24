// -----------------------------------------------------------------------
// <copyright file="CacheService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Cache
{
    using Configuration;
    using Logic;
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using System;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides quick access to frequently utilized resources.
    /// </summary>
    public class CacheService : ICacheService
    {
        private IConnectionMultiplexer _connection;
        private readonly IDataProtector _protector;

        /// <summary>
        /// Initialize a new instance of the <see cref="CacheService"/> class.
        /// </summary>
        public CacheService()
        {
            _protector = new MachineKeyDataProtector(new[] { typeof(CacheService).FullName });
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="CacheService"/> class.
        /// </summary>
        /// <param name="connection">Connection to utilized to communicate with the Redis Cache instance.</param>
        /// <param name="dataProtector">Provides protection for data being cached.</param>
        /// <exception cref="ArgumentNullException">
        /// connection
        /// </exception>
        public CacheService(IConnectionMultiplexer connection, IDataProtector dataProtector)
        {
            connection.AssertNotNull(nameof(connection));
            dataProtector.AssertNotNull(nameof(dataProtector));

            _connection = connection;
            _protector = dataProtector;
        }

        /// <summary>
        /// Removes all entities from the specified cache databae. 
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        public async Task ClearAsync(CacheDatabaseType database)
        {
            EndPoint[] endpoints;
            IServer server;

            try
            {
                if (!IsEnabled)
                {
                    return;
                }

                if (_connection == null)
                {
                    _connection = await ConnectionMultiplexer.ConnectAsync(
                        ApplicationConfiguration.RedisConnection);
                }

                endpoints = _connection.GetEndPoints(true);

                foreach (EndPoint ep in endpoints)
                {
                    server = _connection.GetServer(ep);
                    await server.FlushDatabaseAsync((int)database);
                }
            }
            finally
            {
                endpoints = null;
                server = null;
            }
        }

        /// <summary>
        /// Deletes the entity with specified key.
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        /// <param name="key">The key of the entity to be deleted.</param>
        /// <exception cref="ArgumentException">
        /// key
        /// </exception>
        public async Task DeleteAsync(CacheDatabaseType database, string key)
        {
            key.AssertNotEmpty(nameof(key));

            if (!IsEnabled)
            {
                return;
            }

            IDatabase cache = GetCacheReference(database);
            await cache.KeyDeleteAsync(key);
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

            if (!IsEnabled)
            {
                return null;
            }

            IDatabase cache = GetCacheReference(database);
            RedisValue value = cache.StringGet(key);

            return value.HasValue ?
                JsonConvert.DeserializeObject<TEntity>(_protector.Unprotect(value)) : null;
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
            key.AssertNotEmpty(nameof(key));

            if (!IsEnabled)
            {
                return null;
            }

            IDatabase cache = GetCacheReference(database);
            RedisValue value = await cache.StringGetAsync(key);

            return value.HasValue ? JsonConvert.DeserializeObject<TEntity>(_protector.Unprotect(value)) : null;
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
        public async Task StoreAsync<TEntity>(CacheDatabaseType database, string key, TEntity entity, TimeSpan? expiration = null)
            where TEntity : class
        {
            key.AssertNotEmpty(nameof(key));
            entity.AssertNotNull(nameof(entity));

            if (!IsEnabled)
            {
                return;
            }

            IDatabase cache = await GetCacheReferenceAsync(database);
            await cache.StringSetAsync(
                key, _protector.Protect(JsonConvert.SerializeObject(entity)), expiration);
        }

        /// <summary>
        /// Removes all entities from the specified cache databae. 
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        public void Clear(CacheDatabaseType database)
        {
            EndPoint[] endpoints;
            IServer server;

            try
            {
                if (!IsEnabled)
                {
                    return;
                }

                if (_connection == null)
                {
                    _connection = ConnectionMultiplexer.Connect(
                        ApplicationConfiguration.RedisConnection);
                }

                endpoints = _connection.GetEndPoints(true);

                foreach (EndPoint ep in endpoints)
                {
                    server = _connection.GetServer(ep);
                    server.FlushDatabase((int)database);
                }
            }
            finally
            {
                endpoints = null;
                server = null;
            }
        }

        /// <summary>
        /// Deletes the entity with specified key.
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        /// <param name="key">The key of the entity to be deleted.</param>
        /// <exception cref="ArgumentException">
        /// key
        /// </exception>
        public void Delete(CacheDatabaseType database, string key)
        {
            key.AssertNotEmpty(nameof(key));

            if (!IsEnabled)
            {
                return;
            }

            IDatabase cache = GetCacheReference(database);
            cache.KeyDelete(key);
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
        public void Store<TEntity>(CacheDatabaseType cacheDatabase, string key, TEntity entity, TimeSpan? expiration = null)
            where TEntity : class
        {
            key.AssertNotEmpty(nameof(key));
            entity.AssertNotNull(nameof(entity));

            if (!IsEnabled)
            {
                return;
            }

            IDatabase cache = GetCacheReference(cacheDatabase);
            cache.StringSet(
                key, _protector.Protect(JsonConvert.SerializeObject(entity)), expiration);
        }

        /// <summary>
        /// Gets a boolean value indicating whether or not cache is enabled. 
        /// </summary>
        private bool IsEnabled => string.IsNullOrEmpty(ApplicationConfiguration.RedisConnection) ? false : true;

        /// <summary>
        /// Obtains a reference to the specified cache database.
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        /// <returns>A reference to the appropriate cache database.</returns>
        private IDatabase GetCacheReference(CacheDatabaseType database)
        {
            if (_connection != null)
            {
                return _connection.GetDatabase((int)database);
            }

            try
            {
                _connection = ConnectionMultiplexer.Connect(
                    ApplicationConfiguration.RedisConnection);

                return _connection.GetDatabase((int)database);

            }
            catch (RedisConnectionException ex)
            {
                throw new CacheException(Resources.RedisCacheConnectionException, ex);
            }
        }

        /// <summary>
        /// Obtains a reference to the specified cache database.
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        /// <returns>A reference to the appropriate cache database.</returns>
        private async Task<IDatabase> GetCacheReferenceAsync(CacheDatabaseType database)
        {
            if (_connection != null)
            {
                return _connection.GetDatabase((int)database);
            }

            try
            {
                _connection = await ConnectionMultiplexer.ConnectAsync(
                    ApplicationConfiguration.RedisConnection);

                return _connection.GetDatabase((int)database);

            }
            catch (RedisConnectionException ex)
            {
                throw new CacheException(Resources.RedisCacheConnectionException, ex);
            }
        }
    }
}