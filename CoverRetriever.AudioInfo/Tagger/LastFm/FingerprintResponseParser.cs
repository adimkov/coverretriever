// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FingerprintResponseParser.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Parser of fingerprint response from last.fm  
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo.Tagger.LastFm
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    /// <summary>
    /// Parser of fingerprint response from last.fm.
    /// </summary>
    public class FingerprintResponseParser : ResponseParser
    {
        /// <summary>
        /// The track element.
        /// </summary>
        private const string TrackElement = "track";

        /// <summary>
        /// The artist element.
        /// </summary>
        private const string ArtistElement = "artist";

        /// <summary>
        /// The Name element.
        /// </summary>
        private const string NameElement = "name";

        /// <summary>
        /// Backing field for SuggestedTrackNames.
        /// </summary>
        private readonly IList<string> _suggestedTrackNames = new List<string>();

        /// <summary>
        /// Backing field for SuggestedArtists.
        /// </summary>
        private readonly IList<string> _suggestedArtists = new List<string>(); 

        /// <summary>
        /// Gets the suggested track names.
        /// </summary>
        public IEnumerable<string> SuggestedTrackNames
        {
            get
            {
                return _suggestedTrackNames;
            }
        }

        /// <summary>
        /// Gets the suggested artists.
        /// </summary>
        public IEnumerable<string> SuggestedArtists
        {
            get
            {
                return _suggestedArtists;
            } 
        }

        /// <summary>
        /// Parses the specified response.
        /// </summary>
        /// <param name="response">The response.</param>
        public override void Parse(XDocument response)
        {
            base.Parse(response);
            _suggestedArtists.Clear();
            _suggestedTrackNames.Clear();

            if (IsSuccess)
            {
                foreach (var track in response.Descendants(TrackElement))
                {
                    var nameElement = track.Element(NameElement);
                    if (nameElement != null && !_suggestedTrackNames.Contains(nameElement.Value))
                    {
                        _suggestedTrackNames.Add(nameElement.Value);
                    }    
                }

                foreach (var artist in response.Descendants(ArtistElement))
                {
                    var nameElement = artist.Element(NameElement);
                    if (nameElement != null && !_suggestedArtists.Contains(nameElement.Value))
                    {
                        _suggestedArtists.Add(nameElement.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Clear the response.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            _suggestedArtists.Clear();
            _suggestedTrackNames.Clear();
        }
    }
}