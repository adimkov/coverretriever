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
    using System;

    /// <summary>
    /// Contract to provide audio file summary.
    /// </summary>
    public interface IMetaProvider : IEquatable<IMetaProvider>
    {
        /// <summary>
        /// Gets a value indicating whether Meta Data empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets a value indicating whether this file is changed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this file is changed; otherwise, <c>false</c>.
        /// </value>
        bool IsDirty { get; }

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

        /// <summary>
        /// Copies metadata from specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        void CopyFrom(IMetaProvider source);
    }
}
