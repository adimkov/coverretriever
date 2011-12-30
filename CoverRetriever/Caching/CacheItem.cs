// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheItem.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Caching
{
    using System;

    /// <summary>
    /// The cache item.
    /// </summary>
    public struct CacheItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheItem"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="liveTime">The live time.</param>
        public CacheItem(object value, DateTime liveTime)
            : this()
        {
            Value = value;
            LiveTime = liveTime;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value { get; private set; }

        /// <summary>
        /// Gets the liveTime.
        /// </summary>
        /// <value>
        /// The liveTime.
        /// </value>
        public DateTime LiveTime { get; private set; }     
    }
}