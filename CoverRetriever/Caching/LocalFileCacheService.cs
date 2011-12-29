// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalFileCacheService.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Store cache onto disk at application folder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Caching
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;

    using CoverRetriever.Common.Infrastructure;

    /// <summary>
    /// Store cache onto disk at application folder.
    /// </summary>
    [Export(typeof(ICacheService))]
    public class LocalFileCacheService : ICacheService
    {
        /// <summary>
        /// Path to cache file.
        /// </summary>
        private readonly string _cacheFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileCacheService"/> class.
        /// </summary>
        public LocalFileCacheService()
            : this(".\\cache.tmp")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileCacheService"/> class.
        /// </summary>
        /// <param name="cacheFilePath">The cache file path.</param>
        [ImportingConstructor]
        public LocalFileCacheService(
            [Import(ConfigurationKeys.CacheFilePath)]string cacheFilePath)
        {
            _cacheFilePath = cacheFilePath;
        }

        /// <summary>
        /// The persistence cache dictionary.
        /// </summary>
        private IDictionary<string, CacheItem> _cache = new Dictionary<string, CacheItem>();

        /// <summary>
        /// The one session cache
        /// </summary>
        private readonly IDictionary<string, CacheItem> _sessionCache = new Dictionary<string, CacheItem>();

        /// <summary>
        /// Flush cache onto disk
        /// </summary>
        private IDisposable _saveTimer;

        /// <summary>
        /// Indicating is cache loaded.
        /// </summary>
        private bool _cacheLoaded;

        /// <summary>
        /// Adds or replace the value into cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="mode">The mode.</param>
        public void Add(string key, object value, CacheMode mode)
        {
            var cacheWithItem = GetCacheContainingKey(key);
            if (cacheWithItem != null)
            {
                cacheWithItem.Remove(key);
            }

            if (mode == CacheMode.OneSession)
            {
                _sessionCache.Add(key, CreateCacheItem(value, mode));
            }
            else
            {
                _cache.Add(key, CreateCacheItem(value, mode));
                PerformFlush();
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        public object this[string key]
        {
            get
            {
                if (!_cacheLoaded)
                {
                    Load();
                }

                return GetCachedValue(key);
            }
        }

        /// <summary>
        /// Flushes the cache to file.
        /// </summary>
        private void Flush()
        {
            try
            {
                using (var cacheStream = File.Open(_cacheFilePath,FileMode.OpenOrCreate))
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(cacheStream, _cache);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("could not store cache file due: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Loads the cache file.
        /// </summary>
        private void Load()
        {
            if (File.Exists(_cacheFilePath))
            {
                try
                {
                    using (var cacheStream = File.Open(_cacheFilePath, FileMode.Open))
                    {
                        var formatter = new BinaryFormatter();
                        _cache = (IDictionary<string, CacheItem>)formatter.Deserialize(cacheStream);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Could not load cache file due: {0}", ex.Message);
                }
                finally
                {
                    _cacheLoaded = true;
                }
            }
        }

        /// <summary>
        /// Gets the live time from <see cref="CacheMode"/>.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        private DateTime GetLiveTime(CacheMode mode)
        {
            switch (mode)
            {
                case CacheMode.Short:
                    return DateTime.UtcNow + TimeSpan.FromSeconds(60);
                case CacheMode.Long:
                    return DateTime.UtcNow + TimeSpan.FromSeconds(60);
                default:
                    return DateTime.MaxValue;
            }
        }

        /// <summary>
        /// Creates the cache item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>The cache record</returns>
        private CacheItem CreateCacheItem(object value, CacheMode mode)
        {
            return new CacheItem(value, GetLiveTime(mode));
        }

        /// <summary>
        /// Gets the cache containing key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The cache or null</returns>
        private IDictionary<string, CacheItem> GetCacheContainingKey(string key)
        {
            if (_cache.ContainsKey(key))
            {
                return _cache;
            }
            
            if (_sessionCache.ContainsKey(key))
            {
                return _sessionCache;
            }
            
            return null;
        }

        /// <summary>
        /// Gets the cached value.
        /// <remarks>
        /// if value expired - value will be removed an returned null.
        /// </remarks>
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Value or null.</returns>
        private object GetCachedValue(string key)
        {
            var cacheWithItem = GetCacheContainingKey(key);
            if (cacheWithItem != null)
            {
                var cacheItem = cacheWithItem[key];

                if (cacheItem.LiveTime > DateTime.UtcNow)
                {
                    return cacheWithItem[key].Value;
                }
                
                cacheWithItem.Remove(key);
                return null;
            }

            return null;
        }

        /// <summary>
        /// Performs the flush cache.
        /// </summary>
        private void PerformFlush()
        {
            if (_saveTimer != null)
            {
                _saveTimer.Dispose();
            }

            _saveTimer = Observable.Timer(TimeSpan.FromSeconds(10)).Take(1).Do(_ => Flush()).Subscribe();
        }
    }
}