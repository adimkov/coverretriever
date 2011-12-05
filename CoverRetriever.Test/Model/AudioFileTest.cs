using System;

using CoverRetriever.AudioInfo;
using CoverRetriever.Model;
using CoverRetriever.Service;

using Moq;

using NUnit.Framework;

namespace CoverRetriever.Test.Model
{
    using System.Linq;

    using CoverRetriever.AudioInfo.Tagger;

    [TestFixture]
    public class AudioFileTest : FileSystemItemBaseTest
    {
        [Test]
        public void Ctr_should_create_instance_of_audio_file_but_not_creates_meta_provider_instance()
        {
            var metaProviderMock = new Mock<IMetaProvider>();
            var lazyMetaProvider = new Lazy<IMetaProvider>(() => metaProviderMock.Object);

            var target = new AudioFile("unit", RootFolder, lazyMetaProvider);

            Assert.That(target, Is.Not.Null);
            Assert.That(lazyMetaProvider.IsValueCreated, Is.False);
        }

        [Test]
        public void Ctr_should_create_instance_of_audio_file_but_not_creates_meta_provider_instance_Organizer()
        {
            var metaProviderMock = new Mock<IMetaProvider>();
            var coverOrginizerMock = new Mock<DirectoryCoverOrganizer>();
            var lazyMetaProvider = new Lazy<IMetaProvider>(() => metaProviderMock.Object);

            var target = new AudioFile("unit", RootFolder, lazyMetaProvider, coverOrginizerMock.Object);

            Assert.That(target, Is.Not.Null);
            Assert.That(target.DirectoryCover, Is.Not.Null);
            Assert.That(lazyMetaProvider.IsValueCreated, Is.False);
        }

        [Test]
        public void GetAlbuum_should_create_meta_provider_and_return_album_info()
        {
            var metaProviderMock = new Mock<IMetaProvider>();
            metaProviderMock.Setup(x => x.GetAlbum()).Returns("Это_все");

            var lazyMetaProvider = new Lazy<IMetaProvider>(() => metaProviderMock.Object);

            var target = new AudioFile("unit", RootFolder, lazyMetaProvider);

            Assert.That(target, Is.Not.Null);
            Assert.That(lazyMetaProvider.IsValueCreated, Is.False);
            Assert.That(target.Album, Is.EqualTo("Это_все"));
            Assert.That(lazyMetaProvider.IsValueCreated, Is.True);
        }

        [Test]
        public void GetAlbuum_should_create_meta_provider_and_return_artist_info()
        {
            var metaProviderMock = new Mock<IMetaProvider>();
            metaProviderMock.Setup(x => x.GetArtist()).Returns("DDT");

            var lazyMetaProvider = new Lazy<IMetaProvider>(() => metaProviderMock.Object);

            var target = new AudioFile("unit", RootFolder, lazyMetaProvider);

            Assert.That(target, Is.Not.Null);
            Assert.That(lazyMetaProvider.IsValueCreated, Is.False);
            Assert.That(target.Artist, Is.EqualTo("DDT"));
            Assert.That(lazyMetaProvider.IsValueCreated, Is.True);
        }

        [Test]
        public void GetAlbuum_should_create_meta_provider_and_return_year()
        {
            var metaProviderMock = new Mock<IMetaProvider>();
            metaProviderMock.Setup(x => x.GetYear()).Returns("1995");

            var lazyMetaProvider = new Lazy<IMetaProvider>(() => metaProviderMock.Object);

            var target = new AudioFile("unit", RootFolder, lazyMetaProvider);

            Assert.That(target, Is.Not.Null);
            Assert.That(lazyMetaProvider.IsValueCreated, Is.False);
            Assert.That(target.Year, Is.EqualTo("1995"));
            Assert.That(lazyMetaProvider.IsValueCreated, Is.True);
        }

        [Test]
        public void GetAlbuum_should_create_meta_provider_and_return_track_name()
        {
            var metaProviderMock = new Mock<IMetaProvider>();
            metaProviderMock.Setup(x => x.GetTrackName()).Returns("Aktrisa_vesna");

            var lazyMetaProvider = new Lazy<IMetaProvider>(() => metaProviderMock.Object);

            var target = new AudioFile("unit", RootFolder, lazyMetaProvider);

            Assert.That(target, Is.Not.Null);
            Assert.That(lazyMetaProvider.IsValueCreated, Is.False);
            Assert.That(target.TrackName, Is.EqualTo("Aktrisa_vesna"));
            Assert.That(lazyMetaProvider.IsValueCreated, Is.True);
        }

        [Test]
        public void Should_return_directori_cover_organizer()
        {
            var metaProviderMock = new Mock<IMetaProvider>().As<ICoverOrganizer>();
            var lazyMetaProvider = new Lazy<IMetaProvider>(() => (IMetaProvider)metaProviderMock.Object);
            var directoryCoverMock = new Mock<DirectoryCoverOrganizer>().As<ICoverOrganizer>();

            var target = new AudioFile("unit", RootFolder, lazyMetaProvider, (DirectoryCoverOrganizer)directoryCoverMock.Object);

            Assert.That(target.DirectoryCover, Is.EqualTo(directoryCoverMock.Object));
        }

        [Test]
        public void Should_return_audio_frame_cover_organizer()
        {
            var metaProviderMock = new Mock<IMetaProvider>().As<ICoverOrganizer>();
            var lazyMetaProvider = new Lazy<IMetaProvider>(() => (IMetaProvider)metaProviderMock.Object);
            var directoryCoverMock = new Mock<DirectoryCoverOrganizer>().As<ICoverOrganizer>();

            var target = new AudioFile("unit", RootFolder, lazyMetaProvider, (DirectoryCoverOrganizer)directoryCoverMock.Object);

            Assert.That(target.FrameCover, Is.EqualTo(metaProviderMock.Object));
        }

        [Test]
        public void Should_assign_tagger_to_audio_file()
        {
            var metaProviderMock = new Mock<IMetaProvider>();
            metaProviderMock.Setup(x => x.GetTrackName()).Returns("Aktrisa_vesna");
            var lazyMetaProvider = new Lazy<IMetaProvider>(() => metaProviderMock.Object);
            var tagger = new Mock<ITagger>();
            tagger.Setup(x => x.LoadTagsForAudioFile(It.IsAny<string>()))
                .Returns(Observable.Return(new Unit()));

            var target = new AudioFile("unit", RootFolder, lazyMetaProvider);

            target.AssignTagger(tagger.Object).Run();

            Assert.That(target.MetaProvider, Is.EqualTo(tagger.Object));
        }
    }
}
