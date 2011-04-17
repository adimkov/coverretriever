
using System;
using System.Collections.Generic;

namespace CoverRetriever.Model
{
	/// <summary>
	/// Latest available version of application
	/// </summary>
	public class RevisionVersion : IComparable, IComparable<RevisionVersion>
	{
		public RevisionVersion(Version version, IEnumerable<string> comment)
		{
			Version = version;
			Comment = comment;
		}

		public Version Version { get; set; }
		
		/// <summary>
		/// Build comments
		/// </summary>
		public IEnumerable<string> Comment { get; set; }

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <returns>
		/// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public int CompareTo(RevisionVersion other)
		{
			return Version.CompareTo(other.Version);
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
			var revisionVersion = obj as RevisionVersion;
			if (revisionVersion != null)
			{
				return Version.CompareTo(revisionVersion);
			}
			return 1;
		}
	}
}