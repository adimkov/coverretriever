// -------------------------------------------------------------------------------------------------
// <copyright file="SaveSettings.cs" author="Anton Dimkov">
//     Copyright (c) Anton Dimkov 2013. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------
namespace CoverRetriever.AudioInfo
{
    /// <summary>
    /// Declaration of the <see cref="SaveSettings"/> class.
    /// </summary>
    public class SaveSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SaveSettings"/> is title.
        /// </summary>
        /// <value>
        ///   <c>true</c> if title; otherwise, <c>false</c>.
        /// </value>
        public bool TrackName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SaveSettings" /> is album.
        /// </summary>
        /// <value>
        ///   <c>true</c> if album; otherwise, <c>false</c>.
        /// </value>
        public bool Album { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SaveSettings"/> is artist.
        /// </summary>
        /// <value>
        ///   <c>true</c> if artist; otherwise, <c>false</c>.
        /// </value>
        public bool Artist { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SaveSettings"/> is year.
        /// </summary>
        /// <value>
        ///   <c>true</c> if year; otherwise, <c>false</c>.
        /// </value>
        public bool Year { get; set; }
    }
}