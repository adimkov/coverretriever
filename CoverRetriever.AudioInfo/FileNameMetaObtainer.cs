// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileNameMetaObtainer.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Obtain Meta info from file name.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Obtain Meta info from file name.
    /// </summary>
    public class FileNameMetaObtainer
    {
        /// <summary>
        /// The artist text block.
        /// </summary>
        private const string ArtistBlock = "%artist%";

        /// <summary>
        /// The title text block.
        /// </summary>
        private const string TitleBlock = "%title%";

        /// <summary>
        /// The track number text block.
        /// </summary>
        private const string TrackNumberBlock = "%track%";

        /// <summary>
        /// Regular expression of group.
        /// </summary>
        private const string RegexGroup = @"(?<{0}>.+)";

        /// <summary>
        /// Regular expression of digits group.
        /// </summary>
        private const string DigitGroup = @"(?<{0}>\d+)";

        /// <summary>
        /// Collection of file name parsers.
        /// </summary>
        private IEnumerable<PrioritizedRegex> _fileNameParsers;

        /// <summary>
        /// Backing field Album property.
        /// </summary>
        private string _album;

        /// <summary>
        /// Backing field Artist property.
        /// </summary>
        private string _artist;

        /// <summary>
        /// Backing field Year property.
        /// </summary>
        private string _year;

        /// <summary>
        /// Backing field  property.
        /// </summary>
        private string _trackName;

        /// <summary>
        /// Backing field TrackNumber property.
        /// </summary>
        private string _trackNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileNameMetaObtainer"/> class.
        /// </summary>
        public FileNameMetaObtainer()
        {
            ParsePatterns = new[]
                {
                    "%artist%-%title%", "%title%", "%track%.%artist%-%title%", "%track% %artist%-%title%",
                    "%track%.%title%", "%track% - %title%", "%track%-%title%",
                };

            PrepareRegexPatterns(ParsePatterns);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileNameMetaObtainer"/> class.
        /// </summary>
        /// <param name="parsePatterns">The parse patterns.</param>
        public FileNameMetaObtainer(params string[] parsePatterns)
        {
            ParsePatterns = parsePatterns;

            PrepareRegexPatterns(ParsePatterns);
        }

        /// <summary>
        /// Gets pattern list to parse a file to obtain info.
        /// <remarks>
        ///     Search performing from first to last pattern in case pattern doesn't match.
        /// </remarks>
        /// </summary>
        public string[] ParsePatterns { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is metadata empty.
        /// </summary>
        public virtual bool IsEmpty
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Obtain Audio info from file name and set self state.
        /// <remarks>
        /// Will be used first pattern with maximum matches.
        /// </remarks>
        /// </summary>
        /// <param name="fileName">File name to parse.</param>
        public void ParseFileName(string fileName)
        {
            var groupedRegexByMatches =
                _fileNameParsers.Where(parser => parser.Regex.IsMatch(fileName)).GroupBy(
                    parser => parser.Regex.Match(fileName).Groups.Count);

            var maxMatches = groupedRegexByMatches.Max(x => x.Key);
            var prioritizedRegexes = groupedRegexByMatches.Single(x => x.Key == maxMatches).ToList();
            prioritizedRegexes.Sort();
            var regexWithMaxMatches = prioritizedRegexes.First();

            var groups = regexWithMaxMatches.Regex.Match(fileName).Groups;
            _artist = groups[TrimTemplateSymbol(ArtistBlock)].Value.Trim();
            _trackName = groups[TrimTemplateSymbol(TitleBlock)].Value.Trim();
            _trackNumber = groups[TrimTemplateSymbol(TrackNumberBlock)].Value.Trim();
        }

        /// <summary>
        /// Get an album name.
        /// </summary>
        /// <returns>The album name.</returns>
        public virtual string GetAlbum()
        {
            return _album;
        }

        /// <summary>
        /// Get an artist.
        /// </summary>
        /// <returns>The artist name.</returns>
        public virtual string GetArtist()
        {
            return _artist;
        }

        /// <summary>
        /// Get year of album.
        /// </summary>
        /// <returns>The year of album release.</returns>
        public virtual string GetYear()
        {
            return _year;
        }

        /// <summary>
        /// Get name  of track.
        /// </summary>
        /// <returns>The name of track.</returns>
        public virtual string GetTrackName()
        {
            return _trackName;
        }

        /// <summary>
        /// Trims the template symbol.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Trimmed string.</returns>
        private static string TrimTemplateSymbol(string input)
        {
            return input.Trim('%');
        }

        /// <summary>
        /// Prepares the regex patterns.
        /// </summary>
        /// <param name="parsePatterns">The parse patterns.</param>
        private void PrepareRegexPatterns(string[] parsePatterns)
        {
            var regexList = new List<PrioritizedRegex>(parsePatterns.Length);
            foreach (var pattern in parsePatterns)
            {
                var prority = 0;
                if (parsePatterns.Contains(TrackNumberBlock))
                {
                    prority++;
                }

                regexList.Add(new PrioritizedRegex(new Regex(BuildRegexp(pattern), RegexOptions.IgnoreCase), prority));
            }
            _fileNameParsers = regexList;
        }

        /// <summary>
        /// Builds the regexp.
        /// </summary>
        /// <param name="parsePattern">The parse pattern.</param>
        /// <returns>Built regular expression.</returns>
        private string BuildRegexp(string parsePattern)
        {
            var shieldSpecSymbols = parsePattern.Replace(".", @"\.");
            var trackNumberGroup = DigitGroup.FormatString(TrimTemplateSymbol(TrackNumberBlock));
            var titleGroup = RegexGroup.FormatString(TrimTemplateSymbol(TitleBlock));
            var artistGroup = RegexGroup.FormatString(TrimTemplateSymbol(ArtistBlock));

            return
                shieldSpecSymbols.Replace(TrackNumberBlock, trackNumberGroup).Replace(ArtistBlock, artistGroup).Replace(
                    TitleBlock, titleGroup);
        }
    }

    /// <summary>
    /// Prioritized regular expression.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder",
        Justification = "Reviewed. Suppression is OK here.")]
    internal struct PrioritizedRegex : IComparable<PrioritizedRegex>, IComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrioritizedRegex"/> struct.
        /// </summary>
        /// <param name="regex">The regex.</param>
        public PrioritizedRegex(Regex regex)
            : this()
        {
            Regex = regex;
            Priority = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrioritizedRegex"/> struct.
        /// </summary>
        /// <param name="regex">The regex.</param>
        /// <param name="priority">The priority.</param>
        public PrioritizedRegex(Regex regex, int priority)
            : this()
        {
            Regex = regex;
            Priority = priority;
        }

        /// <summary>
        /// Gets or sets the regular expression.
        /// </summary>
        /// <value>
        /// The regular expression.
        /// </value>
        public Regex Regex { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public int Priority { get; set; }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(PrioritizedRegex other)
        {
            return Priority.CompareTo(other.Priority);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>. 
        /// </returns>
        /// <param name="obj">An object to compare with this instance. </param><exception cref="T:System.ArgumentException"><paramref name="obj"/> is not the same type as this instance. </exception><filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            return CompareTo((PrioritizedRegex)obj);
        }
    }
}