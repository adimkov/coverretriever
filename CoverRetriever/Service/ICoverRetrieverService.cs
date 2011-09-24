// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICoverRetrieverService.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Service to grab covers from web
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Service
{
    using System;
    using System.Collections.Generic;

    using CoverRetriever.Model;

    /// <summary>
    /// Service to grab covers from web.
    /// </summary>
    public interface ICoverRetrieverService
    {
        /// <summary>
        /// Get cover for audio track.
        /// </summary>
        /// <param name="artist">Artist name.</param>
        /// <param name="album">Album name.</param>
        /// <param name="coverCount">Count of cover. Range 1-8.</param>
        /// <returns>Found covers.</returns>
        IObservable<IEnumerable<RemoteCover>> GetCoverFor(string artist, string album, int coverCount);
    }
}