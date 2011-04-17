using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml.Linq;

using CoverRetriever.Model;

namespace CoverRetriever.Service
{
	[Export(typeof(RevisionVersionParser))]
	public class RevisionVersionParser
	{
		private const string VersionElement = "Version";
		private const string ChangeElement = "Change";
		private const string NumberAttribute = "number";

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