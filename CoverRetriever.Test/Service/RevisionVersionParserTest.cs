using System.Linq;
using System.Xml.Linq;

using CoverRetriever.Service;

using NUnit.Framework;

namespace CoverRetriever.Test.Service
{
	[TestFixture]
	public class RevisionVersionParserTest
	{
		[Test]
		public void ParseVersionHistory_should_parse_single_node()
		{
			using (var stream = ResourceUtils.GetTestInputStream("Versions_single.xml"))
			{
				var target = new RevisionVersionParser();
				var result = target.ParseVersionHistory(XDocument.Load(stream));
				Assert.That(result.Count(), Is.EqualTo(1));

				var firstVersion = result.First();
				Assert.That(firstVersion.Version.Major, Is.EqualTo(1));
				Assert.That(firstVersion.Version.Minor, Is.EqualTo(0));
				Assert.That(firstVersion.Version.Build, Is.EqualTo(0));
				Assert.That(firstVersion.Version.Revision, Is.EqualTo(0));

				Assert.That(firstVersion.Comment.Count(), Is.EqualTo(2));
				Assert.That(firstVersion.Comment.First(), Is.EqualTo("Some change #1"));
				Assert.That(firstVersion.Comment.Last(), Is.EqualTo("Some change #2"));
			}
		}

		[Test]
		public void ParseVersionHistory_should_parse_two_nodes()
		{
			using (var stream = ResourceUtils.GetTestInputStream("Versions_couple.xml"))
			{
				var target = new RevisionVersionParser();
				var result = target.ParseVersionHistory(XDocument.Load(stream));
				Assert.That(result.Count(), Is.EqualTo(2));

				var firstVersion = result.First();
				Assert.That(firstVersion.Version.Major, Is.EqualTo(2));
				Assert.That(firstVersion.Version.Minor, Is.EqualTo(5));
				Assert.That(firstVersion.Version.Build, Is.EqualTo(13));
				Assert.That(firstVersion.Version.Revision, Is.EqualTo(3));

				Assert.That(firstVersion.Comment.Count(), Is.EqualTo(3));
				Assert.That(firstVersion.Comment.First(), Is.EqualTo("Some change #1"));
				Assert.That(firstVersion.Comment.Skip(1).Take(1).Single(), Is.EqualTo("Some change #2"));
				Assert.That(firstVersion.Comment.Last(), Is.EqualTo("Some change #3"));
			}
		}
	}
}