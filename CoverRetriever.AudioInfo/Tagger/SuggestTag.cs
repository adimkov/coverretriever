// -------------------------------------------------------------------------------------------------
// <copyright file="SuggestTag.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Provide tags for audio file
// </summary>
// -------------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo.Tagger
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Result of tag suggestion.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Reviewed. Suppression is OK here.")]
    public class SuggestTag : IMetaProvider
    {
        /// <summary>
        /// The suggested album.
        /// </summary>
        private string album;

        /// <summary>
        /// The suggested artist.
        /// </summary>
        private string artist;

        /// <summary>
        /// The suggested year.
        /// </summary>
        private string year;

        /// <summary>
        /// The suggested trackName.
        /// </summary>
        private string trackName;

        /// <summary>
        /// Gets a value indicating whether Meta Data empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(album) &&
                    string.IsNullOrEmpty(artist) &&
                    string.IsNullOrEmpty(trackName) &&
                    string.IsNullOrEmpty(year);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this file is changed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this file is changed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirty
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets suggested album name.
        /// </summary>
        public string Album
        {
            get
            {
                return album;
            }

            set
            {
                album = value;
            }
        }

        /// <summary>
        /// Gets suggested artist.
        /// </summary>
        public string Artist
        {
            get
            {
                return artist;
            }

            set
            {
                artist = value;
            }
        }

        /// <summary>
        /// Gets or sets suggested year of album.
        /// </summary>
        public string Year
        {
            get
            {
                return year;
            }

            set
            {
                year = value;
            }
        }

        /// <summary>
        /// Gets suggested name of track.
        /// </summary>
        public string TrackName
        {
            get
            {
                return trackName;
            }

            set
            {
                trackName = value;
            }
        }

        /// <summary>
        /// Saves tags into file instance.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.NotSupportedException">Suggested tag does not support this operation</exception>
        public void Save(SaveSettings settings)
        {
            throw new NotSupportedException("Suggested tag does not support this operation");
        }

        /// <summary>
        /// Copies metadata from specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.NotSupportedException">Suggested tag does not support this operation</exception>
        public void CopyFrom(IMetaProvider source, SaveSettings settings)
        {
            throw new NotSupportedException("Suggested tag does not support this operation");
        }

        /// <summary>
        /// Equals the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>True if tags are equals.</returns>
        public bool Equals(IMetaProvider other)
        {
            return Album == other.Album &&
                Artist == other.Artist &&
                Year == other.Year &&
                TrackName == other.TrackName;
        }
    }
}