
using ADSBackend.Models.CacheViewModels;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace ADSBackend.Services
{
    /// <summary>
    /// Service for caching data in memory.
    /// </summary>
    public class Cache
    {
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cache"/> class.
        /// </summary>
        /// <param name="cache">The memory cache instance.</param>
        public Cache(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Asynchronously gets an item from the cache. If the item is not found or expired, it is fetched using the provided function and stored in the cache.
        /// </summary>
        /// <typeparam name="T">The type of the item to get.</typeparam>
        /// <param name="key">The key of the cache item.</param>
        /// <param name="dataFetchFunction">The function to fetch the data if it's not in the cache.</param>
        /// <param name="lifeSpan">The lifespan of the cache item.</param>
        /// <returns>The cached item.</returns>
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataFetchFunction, TimeSpan lifeSpan)
        {
            T oRet;

            var cachedData = _cache.Get<CacheItem<T>>(key);
            if (cachedData == null || cachedData.IsExpired())
            {
                var sourceResponse = await dataFetchFunction();
                if (sourceResponse != null)
                {
                    // store in in-memory cache
                    cachedData = new CacheItem<T>
                    {
                        Created = DateTime.Now,
                        LifeSpan = lifeSpan,
                        Data = sourceResponse
                    };
                    _cache.Set(key, cachedData);

                    oRet = sourceResponse;
                }
                else
                {
                    // an error occurred while trying to retrieve from the source
                    // return the default for the generic type
                    oRet = default(T);
                }
            }
            else
            {
                // all good, use cached data
                oRet = cachedData.Data;
            }

            return oRet;
        }

        /// <summary>
        /// Synchronously gets an item from the cache. If the item is not found or expired, it is fetched using the provided function and stored in the cache.
        /// </summary>
        /// <typeparam name="T">The type of the item to get.</typeparam>
        /// <param name="key">The key of the cache item.</param>
        /// <param name="dataFetchFunction">The function to fetch the data if it's not in the cache.</param>
        /// <param name="lifeSpan">The lifespan of the cache item.</param>
        /// <returns>The cached item.</returns>
        public T Get<T>(string key, Func<T> dataFetchFunction, TimeSpan lifeSpan)
        {
            T oRet;

            var cachedData = _cache.Get<CacheItem<T>>(key);
            if (cachedData == null || cachedData.IsExpired())
            {
                var sourceResponse = dataFetchFunction();
                if (sourceResponse != null)
                {
                    // store in in-memory cache
                    cachedData = new CacheItem<T>
                    {
                        Created = DateTime.Now,
                        LifeSpan = lifeSpan,
                        Data = sourceResponse
                    };
                    _cache.Set(key, cachedData);

                    oRet = sourceResponse;
                }
                else
                {
                    // an error occurred while trying to retrieve from the source
                    // return the default for the generic type
                    oRet = default(T);
                }
            }
            else
            {
                // all good, use cached data
                oRet = cachedData.Data;
            }

            return oRet;
        }
    }
}