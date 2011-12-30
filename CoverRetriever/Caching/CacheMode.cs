// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheMode.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Cache modes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Caching
{
    /// <summary>
    /// Cache modes.
    /// </summary>
    public enum CacheMode
    {
        /// <summary>
        /// Short life time mode.
        /// </summary>
        Short,

        /// <summary>
        /// Long life time mode.
        /// </summary>
        Long,

        /// <summary>
        /// Cache that live during one session.
        /// </summary>
        OneSession,

        /// <summary>
        /// Never expired cache.
        /// </summary>
        NotExpired,    
    }
}