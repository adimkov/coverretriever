// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackInfoResponseParser.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Parses response of trackInfo from last.fm service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo.Tagger.LastFm
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    /// <summary>
    /// Parses response of trackInfo from last.fm service.
    /// </summary>
    public class TrackInfoResponseParser : ResponseParser
    {
        /// <summary>
        /// The album element.
        /// </summary>
        public const string AlbumElement = "album";

        /// <summary>
        /// The title element.
        /// </summary>
        public const string TitleElement = "title";

        /// <summary>
        /// The backing field for SuggestedAlbums.
        /// </summary>
        private readonly IList<string> _sugesstedAlbums = new List<string>();

        /// <summary>
        /// Gets the suggested albums.
        /// </summary>
        public IEnumerable<string> SuggestedAlbums
        {
            get
            {
                return _sugesstedAlbums;
            }
        }

        /// <summary>
        /// Parses the specified response.
        /// </summary>
        /// <param name="response">The response.</param>
        public override void Parse(XDocument response)
        {
            _sugesstedAlbums.Clear();
            base.Parse(response);
            if (IsSuccess)
            {
                foreach (var albums in response.Descendants(AlbumElement))
                {
                    var nameElement = albums.Element(TitleElement);
                    if (nameElement != null && !_sugesstedAlbums.Contains(nameElement.Value))
                    {
                        _sugesstedAlbums.Add(nameElement.Value);
                    }
                }
            }
        }
    }
}