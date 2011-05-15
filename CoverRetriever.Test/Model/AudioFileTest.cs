using System;
using System.Linq;

using CoverRetriever.AudioInfo;
using CoverRetriever.Model;
using CoverRetriever.Service;

using Moq;

using NUnit.Framework;

namespace CoverRetriever.Test.Model
{
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
		public void Ctr_should_create_instance_of_audio_file_but_not_creates_meta_provider_instance_init_CoverOrganizer()
		{
			var metaProviderMock = new Mock<IMetaProvider>();
			var coverOrginizerMock = new Mock<DirectoryCoverOrganizer>().As<ICoverOrganizer>();
			coverOrginizerMock.Setup(x => x.Init(It.IsAny<AudioFile>())).AtMostOnce();

			var lazyMetaProvider = new Lazy<IMetaProvider>(() => metaProviderMock.Object);

			var target = new AudioFile("unit", RootFolder, lazyMetaProvider, Enumerable.Repeat(coverOrginizerMock.Object, 1));

			Assert.That(target, Is.Not.Null);
			Assert.That(target.DirectoryCover, Is.Not.Null);
			Assert.That(lazyMetaProvider.IsValueCreated, Is.False);
			coverOrginizerMock.Verify();
		}

		[Test]
		public void GetAlbuum_should_create_mett_provider_and_return_album_info()
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
		public void GetAlbuum_should_create_mett_provider_and_return_artist_info()
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
		public void GetAlbuum_should_create_mett_provider_and_return_year()
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
		public void GetAlbuum_should_create_mett_provider_and_return_track_name()
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

	}
}
