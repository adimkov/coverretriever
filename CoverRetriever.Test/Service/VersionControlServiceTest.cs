
using System;
using System.Linq;
using System.Xml.Linq;

using CoverRetriever.Model;
using CoverRetriever.Service;

using Moq;

using NUnit.Framework;

namespace CoverRetriever.Test.Service
{
	[TestFixture]
	public class VersionControlServiceTest
	{
		[Test]
		public void GetLatestVersion_should_download_xml_of_latest_version_application()
		{
			var conStr = "http://adimkov.users.sourceforge.net/CoverRetrieverAssets/Versions_test.xml";
			var revisionVersionParserMock = new Mock<RevisionVersionParser>();
			revisionVersionParserMock.Setup(x => x.ParseVersionHistory(It.IsAny<XDocument>()))
				.Returns(Enumerable.Empty<RevisionVersion>);

			var target = new HttpVersionControlService(conStr, revisionVersionParserMock.Object);
			target.GetLatestVersion().Run();
			revisionVersionParserMock.VerifyAll();
		}

		[Test]
		public void GetLatestVersion_should_download_xml_of_latest_version_application_and_return_latest_version()
		{
			var conStr = "http://adimkov.users.sourceforge.net/CoverRetrieverAssets/Versions_test.xml";
			
			var revisionVersionParserMock = new Mock<RevisionVersionParser>();
				revisionVersionParserMock.Setup(x => x.ParseVersionHistory(It.IsAny<XDocument>()))
					.Returns(new []
					{
						new RevisionVersion(new Version("0.0.3.5"), Enumerable.Empty<string>()),
						new RevisionVersion(new Version("0.1.3.5"), Enumerable.Empty<string>()),
					});

			var target = new HttpVersionControlService(conStr, revisionVersionParserMock.Object);
			RevisionVersion latest = null;
			target.GetLatestVersion().Run(x => latest = x);
			revisionVersionParserMock.VerifyAll();
			Assert.That(latest.Version.ToString(), Is.EqualTo("0.1.3.5"));
		}
	}
}