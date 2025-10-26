
using System;

namespace ADSBackend.Models.CacheViewModels
{
    /// <summary>
    /// Represents an item stored in the cache.
    /// </summary>
    /// <typeparam name="T">The type of the data being cached.</typeparam>
    public class CacheItem<T>
    {
        /// <summary>
        /// Gets or sets the cached data.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets the creation time of the cache item.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the lifespan of the cache item.
        /// </summary>
        public TimeSpan LifeSpan { get; set; }

        /// <summary>
        /// Determines whether the cache item has expired.
        /// </summary>
        /// <returns><c>true</c> if the item has expired; otherwise, <c>false</c>.</returns>
        public bool IsExpired()
        {
            return Created.Add(LifeSpan) < DateTime.Now;
        }
    }
}