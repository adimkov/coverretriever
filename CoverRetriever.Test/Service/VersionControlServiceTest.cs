
using System;
using System.Xml.Linq;

using CoverRetriever.Model;
using CoverRetriever.Service;

using Moq;

using NUnit.Framework;

namespace CoverRetriever.Test.Service
{
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;

    using Microsoft.Reactive.Testing;

    [TestFixture]
    public class VersionControlServiceTest
    {
        private const string VersionFileLocation =
            "http://adimkov.users.sourceforge.net/CoverRetrieverAssets/Versions_test.xml"; 

        [Test]
        public void GetLatestVersion_should_download_xml_of_latest_version_application()
        {
            var revisionVersionParserMock = new Mock<RevisionVersionParser>();
            revisionVersionParserMock.Setup(x => x.ParseVersionHistory(It.IsAny<XDocument>()))
                .Returns(Enumerable.Empty<RevisionVersion>);

            var target = new HttpVersionControlService(VersionFileLocation, revisionVersionParserMock.Object);
            target.GetLatestVersion().FirstOrDefault();
            revisionVersionParserMock.VerifyAll();
        }

        [Test]
        public void GetLatestVersion_should_download_xml_of_latest_version_application_and_return_latest_version()
        {
            var revisionVersionParserMock = new Mock<RevisionVersionParser>();
                revisionVersionParserMock.Setup(x => x.ParseVersionHistory(It.IsAny<XDocument>()))
                    .Returns(new []
                    {
                        new RevisionVersion(new Version("0.0.3.5"), Enumerable.Empty<string>()),
                        new RevisionVersion(new Version("0.1.3.5"), Enumerable.Empty<string>()),
                    });

            var target = new HttpVersionControlService(VersionFileLocation, revisionVersionParserMock.Object);
            RevisionVersion latest = null;
            target.GetLatestVersion().ForEach(x => latest = x);
            revisionVersionParserMock.VerifyAll();
            Assert.That(latest.Version.ToString(), Is.EqualTo("0.1.3.5"));
        }

        [Test]
        public void Should_call_one_and_push_completed()
        {
            var sheduler = new TestScheduler();
            var revisionVersionParserMock = new Mock<RevisionVersionParser>();
            var target = new HttpVersionControlService(VersionFileLocation, revisionVersionParserMock.Object);

            var observer = sheduler.CreateObserver<RevisionVersion>();
            target.GetLatestVersion().Run(observer);
            sheduler.Start();

            Assert.That(observer.Messages.Count, Is.EqualTo(2));
            Assert.That(observer.Messages[0].Value.Kind, Is.EqualTo(NotificationKind.OnNext));
            Assert.That(observer.Messages[1].Value.Kind, Is.EqualTo(NotificationKind.OnCompleted));
        }
    }
}