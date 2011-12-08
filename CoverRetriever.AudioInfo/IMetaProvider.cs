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
        /// Gets or sets an album name.
        /// </summary>
        string Album { get; set; }

        /// <summary>
        /// Gets or sets an artist.
        /// </summary>
        string Artist { get; set; }

        /// <summary>
        /// Gets or sets year of album.
        /// </summary>
        string Year { get; set; }

        /// <summary>
        /// Gets or sets name of track.
        /// </summary>
        string TrackName { get; set; }

        /// <summary>
        /// Saves tags into file instance.
        /// </summary>
        void Save();
    }
}
