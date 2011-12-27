// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlbumInfoResponseParser.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Parses response of albumInfo from last.fm service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo.Tagger.LastFm
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml.Linq;

    /// <summary>
    /// Parses response of albumInfo from last.fm service.
    /// </summary>
    public class AlbumInfoResponseParser : ResponseParser
    {
        /// <summary>
        /// The release date element.
        /// </summary>
        public const string ReleaseDateElement = "releasedate";

        /// <summary>
        /// Backing field of the SuggestedYears property.
        /// </summary>
        private readonly IList<string> _suggestedYears = new List<string>();

        /// <summary>
        /// Gets the suggested album years.
        /// </summary>
        public IEnumerable<string> SuggestedYears
        {
            get
            {
                return _suggestedYears;
            }
        }

        /// <summary>
        /// Parses the specified response.
        /// </summary>
        /// <param name="response">The response.</param>
        public override void Parse(XDocument response)
        {
            _suggestedYears.Clear();
            base.Parse(response);
            if (IsSuccess)
            {
                try
                {
                    foreach (var releaseDate in response.Descendants(ReleaseDateElement))
                    {
                        if (releaseDate != null && !_suggestedYears.Contains(releaseDate.Value))
                        {
                            var release = DateTime.Parse(releaseDate.Value.Trim());

                            _suggestedYears.Add(release.Year.ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Can not parse release date. {0}", e.ToString());
                }
            }
        }

        /// <summary>
        /// Clear the response.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            _suggestedYears.Clear();
        }
    }
}