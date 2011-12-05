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
        /// Get an album name.
        /// </summary>
        /// <returns>The name of album.</returns>
        string GetAlbum();

        /// <summary>
        /// Gets an artist.
        /// </summary>
        /// <returns>The artist.</returns>
        string GetArtist();

        /// <summary>
        /// Get year of album.
        /// </summary>
        /// <returns>The album year.</returns>
        string GetYear();

        /// <summary>
        /// Gets name of track.
        /// </summary>
        /// <returns>The name of track.</returns>
        string GetTrackName();
    }
}