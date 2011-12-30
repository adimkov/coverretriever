// --------------------------------------------------------------------------------------------------------------------
// <copyright file="adding_new_cache_item.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Test.CacheService
{
    using CoverRetriever.Caching;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Test scenario of adding new cache item
    /// </summary>
    public class adding_new_cache_item : cache_service_runtime
    {
        [Test]
        public void should_add_item_to_cache_dictionary()
        {
            CacheService = EmptyCacheService();
            CacheService.Add("TestKey", 42, CacheMode.NotExpired);

            CacheService["TestKey"].Should().Be(42);
        }

        [Test]
        public void should_override_existing_element()
        {
            CacheService = CacheServiceWithOneElement();

            CacheService.Add("key1", 101, CacheMode.Long);
            CacheService["key1"].Should().Be(101);
        }

        [Test]
        public void should_replace_item_with_any_cache_mode_by_new_item_with_same_key()
        {
            var key = "key1";
            CacheService = EmptyCacheService();

            CacheService.Add(key, 1, CacheMode.Short);
            CacheService[key].Should().Be(1);

            CacheService.Add(key, 2, CacheMode.NotExpired);
            CacheService[key].Should().Be(2);

            CacheService.Add(key, 3, CacheMode.OneSession);
            CacheService[key].Should().Be(3);

            CacheService.Add(key, 4, CacheMode.OneSession);
            CacheService[key].Should().Be(4);
        }
    }
}