﻿// -----------------------------------------------------------------------
// <copyright file="ICacheService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Cache
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides quick access to frequently utilized resources.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Removes all entities from the specified cache database. 
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        Task ClearAsync(CacheDatabaseType database);

        /// <summary>
        /// Removes the specified entity from the cache.
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        /// <param name="key">A unique indentifier for the cache entry.</param>
        /// <exception cref="ArgumentException">
        /// key
        /// </exception>
        Task DeleteAsync(CacheDatabaseType database, string key);

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
        TEntity Fetch<TEntity>(CacheDatabaseType database, string key) where TEntity : class;

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
        Task<TEntity> FetchAsync<TEntity>(CacheDatabaseType database, string key) where TEntity : class;

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
        Task StoreAsync<TEntity>(CacheDatabaseType database, string key, TEntity entity, TimeSpan? expiration = null)
            where TEntity : class;

        /// <summary>
        /// Removes all entities from the specified cache databae. 
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        void Clear(CacheDatabaseType database);

        /// <summary>
        /// Removes the specified entity from the cache.
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        /// <param name="key">A unique indentifier for the cache entry.</param>
        /// <exception cref="ArgumentException">
        /// key
        /// </exception>
        void Delete(CacheDatabaseType database, string key);

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
        void Store<TEntity>(CacheDatabaseType database, string key, TEntity entity, TimeSpan? expiration = null)
            where TEntity : class;
    }
}