// --------------------------------------------------------------------------------------------------------------------
// <copyright file="cache_service_runtime.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Test.CacheService
{
    using CoverRetriever.Caching;
    using NUnit.Framework;

    /// <summary>
    /// Base class of testing of cache service.
    /// </summary>
    [TestFixture]
    public class cache_service_runtime
    {
        /// <summary>
        /// Gets the cache service.
        /// </summary>
        protected LocalFileCacheService CacheService { get; set; }

        /// <summary>
        /// Empty cache service.
        /// </summary>
        protected LocalFileCacheService EmptyCacheService()
        {
            return new LocalFileCacheService();
        }

        /// <summary>
        /// Caches the service with one element.
        /// </summary>
        /// <returns></returns>
        protected LocalFileCacheService CacheServiceWithOneElement()
        {
            var cacheService = new LocalFileCacheService();
            cacheService.Add("key1", 50, CacheMode.NotExpired);

            return cacheService;
        }

        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        [TearDown] 
        public void Cleanup()
        {
            CacheService = null;
        }
    }
}