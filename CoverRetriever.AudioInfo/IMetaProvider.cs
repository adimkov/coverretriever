// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMetaProvider.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Contract to provide audio file summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo
{
    /// <summary>
    /// Contract to provide audio file summary.
    /// </summary>
    public interface IMetaProvider
    {
        /// <summary>
        /// Gets a value indicating whether Meta Data empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets an album name.
        /// </summary>
        string Album { get; }

        /// <summary>
        /// Gets an artist.
        /// </summary>
        string Artist { get; }

        /// <summary>
        /// Gets year of album.
        /// </summary>
        string Year { get; }

        /// <summary>
        /// Gets name of track.
        /// </summary>
        string TrackName { get; }
    }
}