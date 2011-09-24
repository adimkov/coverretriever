// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RevisionVersionParser.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Parser of revision xml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Service
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Xml.Linq;

    using CoverRetriever.Model;

    /// <summary>
    /// Parser of revision xml.
    /// </summary>
    [Export(typeof(RevisionVersionParser))]
    public class RevisionVersionParser
    {
        /// <summary>
        /// Version element.
        /// </summary>
        private const string VersionElement = "Version";

        /// <summary>
        /// Change element.
        /// </summary>
        private const string ChangeElement = "Change";

        /// <summary>
        /// Number attribute.
        /// </summary>
        private const string NumberAttribute = "number";

        /// <summary>
        /// Parses the version history.
        /// </summary>
        /// <param name="versionHistory">The version history.</param>
        /// <returns>Version history.</returns>
        public virtual IEnumerable<RevisionVersion> ParseVersionHistory(XDocument versionHistory)
        {
            var versions = from version in versionHistory.Descendants(VersionElement)
                           let versionNumb = version.Attribute(NumberAttribute).Value
                           let comments = version.Descendants(ChangeElement).Select(x => x.Value.Trim())
                           select new RevisionVersion(new Version(versionNumb), comments);

            return versions;
        }
    }
}