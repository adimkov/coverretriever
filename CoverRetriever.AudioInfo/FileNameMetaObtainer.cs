using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace CoverRetriever.AudioInfo
{
	/// <summary>
	/// Obtain Meta info from file name
	/// </summary>
	public class FileNameMetaObtainer
	{
		private const string ArtistBlock = "%artist%";
		private const string TitleBlock = "%title%";
		private const string TrackNumberBlock = "%track%";
		private const string RegexGroup = @"(?<{0}>.+)";
		private const string DigitGroup = @"(?<{0}>\d+)";

		private IEnumerable<PrioritizedRegex> _fileNameParsers;

		private string _album;
		private string _artist;
		private string _year;
		private string _trackName;
		private string _trackNumber;

		public FileNameMetaObtainer()
		{
			ParsePatterns = new[]
			                	{
			                		"%artist%-%title%", 
									"%title%", 
									"%track%.%artist%-%title%", 
									"%track% %artist%-%title%", 
									"%track%.%title%",
									"%track% - %title%",
									"%track%-%title%",
			                	};
			PrepareRegexPatterns(ParsePatterns);
		}

		public FileNameMetaObtainer(params string[] parsePatterns)
		{
			ParsePatterns = parsePatterns;

			PrepareRegexPatterns(ParsePatterns);
		}

		/// <summary>
		/// Pattern list to parse a file to obtain info.
		/// <remarks>
		/// Search performing from first to last pattern in case pattern doesn't match
		/// </remarks>
		/// </summary>
		public string[] ParsePatterns { get; private set; }

		/// <summary>
		/// Obtain Audio info from file name and set self state.
		/// <remarks>
		/// Will be used first pattern with maximum matches
		/// </remarks>
		/// </summary>
		/// <param name="fileName"></param>
		public void ParseFileName(string fileName)
		{
			var groupedRegexByMatches = _fileNameParsers
				.Where(parser => parser.Regex.IsMatch(fileName))
				.GroupBy(parser => parser.Regex.Match(fileName).Groups.Count);
			
			var maxMatches = groupedRegexByMatches.Max(x => x.Key);
			var prioritizedRegexes = groupedRegexByMatches
				.Single(x => x.Key == maxMatches)
				.ToList();
			prioritizedRegexes.Sort();
			var regexWithMaxMatches = prioritizedRegexes.First();

			var groups = regexWithMaxMatches.Regex.Match(fileName).Groups;
			_artist = groups[TrimTemplateSymbol(ArtistBlock)].Value.Trim();
			_trackName = groups[TrimTemplateSymbol(TitleBlock)].Value.Trim();
			_trackNumber = groups[TrimTemplateSymbol(TrackNumberBlock)].Value.Trim();
		}

		/// <summary>
		/// Indicate is Meta Data empty
		/// </summary>
		public virtual bool IsEmpty
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Get an album name
		/// </summary>
		/// <returns></returns>
		public virtual string GetAlbum()
		{
			return _album;
		}

		/// <summary>
		/// Get an artist
		/// </summary>
		/// <returns></returns>
		public virtual string GetArtist()
		{
			return _artist;
		}

		/// <summary>
		/// Get year of album
		/// </summary>
		/// <returns></returns>
		public virtual string GetYear()
		{
			return _year;
		}

		/// <summary>
		/// Get name  of track
		/// </summary>
		/// <returns></returns>
		public virtual string GetTrackName()
		{
			return _trackName;
		}

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

				regexList.Add(new PrioritizedRegex(
					new Regex(BuildRegexp(pattern), RegexOptions.IgnoreCase), 
					prority));
			}
			_fileNameParsers = regexList;
		}

		private string BuildRegexp(string parsePattern)
		{
			var shieldSpecSymbols = parsePattern.Replace(".", @"\.");
			var trackNumberGroup = String.Format(DigitGroup, TrimTemplateSymbol(TrackNumberBlock));
			var titleGroup = String.Format(RegexGroup, TrimTemplateSymbol(TitleBlock));
			var artistGroup = String.Format(RegexGroup, TrimTemplateSymbol(ArtistBlock));

			return shieldSpecSymbols
				.Replace(TrackNumberBlock, trackNumberGroup)
				.Replace(ArtistBlock, artistGroup)
				.Replace(TitleBlock, titleGroup);
		}

		private static string TrimTemplateSymbol(string input)
		{
			return input.Trim('%');
		}	
	}

	internal struct PrioritizedRegex : IComparable<PrioritizedRegex>, IComparable
	{
		public PrioritizedRegex(Regex regex)
			: this()
		{
			Regex = regex;
			Priority = 0;
		}

		public PrioritizedRegex(Regex regex, int priority)
			: this()
		{
			Regex = regex;
			Priority = priority;
		}

		public Regex Regex { get; set; }
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