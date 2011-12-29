// --------------------------------------------------------------------------------------------------------------------
// <copyright file="reading_cache_item.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Test.CacheService
{
    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Testing scenario when reading cache item
    /// </summary>
    public class reading_cache_item : cache_service_runtime
    {
        [Test]
        public void should_read_value_if_cache_item_exists_and_not_expired()
        {
            CacheService = CacheServiceWithOneElement();

            CacheService["key1"].Should().Be(50);
        }

        [Test]
        public void should_read_null_if_cache_item_does_not_exist_or_expired()
        {
            CacheService = EmptyCacheService();

            CacheService["NotKey"].Should().BeNull();
        }
    }
}