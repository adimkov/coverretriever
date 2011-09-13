// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RevisionVersion.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  The version of the application
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Model
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The version of the application.
    /// </summary>
    public class RevisionVersion : IComparable, IComparable<RevisionVersion>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RevisionVersion"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="comment">The comment.</param>
        /// <remarks>
        /// Build notation: Major.Minor.Revision.Build.
        /// </remarks>
        public RevisionVersion(Version version, IEnumerable<string> comment)
        {
            Version = version;
            Comment = comment;
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the build comments.
        /// </summary>
        /// <value>The comment.</value>
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