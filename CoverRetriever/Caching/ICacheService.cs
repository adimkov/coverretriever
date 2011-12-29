// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICacheService.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Contract to access local cache.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Caching
{
    /// <summary>
    /// Contract to access local cache.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Adds or replace the value into cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="mode">The mode.</param>
        void Add(string key, object value, CacheMode mode);

        /// <summary>
        /// Resets this instance.
        /// </summary>
        void Reset();

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        object this[string key] { get; }
    }
}