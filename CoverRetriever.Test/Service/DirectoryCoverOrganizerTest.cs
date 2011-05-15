using System;
using System.Linq;

using CoverRetriever.AudioInfo;
using CoverRetriever.Model;
using CoverRetriever.Service;

using Moq;

using NUnit.Framework;

namespace CoverRetriever.Test.Service
{
	[TestFixture]
	public class DirectoryCoverOrganizerTest
	{
		private const string PathToFolderWithoutCover = @"g:\������\���\[2006] - Family CD2\";
		private const string PathToFolderWithCover = @"g:\������\���\[1991] - �������\";

		[Test]
		public void IsCoverExists_should_return_true()
		{
			var metaProviderMock = new Mock<IMetaProvider>();
			var rootFolder = new RootFolder(PathToFolderWithCover);

			var target = new DirectoryCoverOrganizer();
			var audioFile = new AudioFile("04 - �����.mp3", rootFolder, new Lazy<IMetaProvider>(() => metaProviderMock.Object), Enumerable.Repeat<ICoverOrganizer>(target, 1));

			Assert.IsTrue(target.IsCoverExists());
			Assert.That(target.CoverName, Is.EqualTo("cover.jpg"));
		}

		[Test]
		public void IsCoverExists_should_return_false()
		{
			var metaProviderMock = new Mock<IMetaProvider>();
			var rootFolder = new RootFolder(PathToFolderWithoutCover);

			var target = new DirectoryCoverOrganizer();
			var audioFile = new AudioFile("04 - �����.mp3", rootFolder, new Lazy<IMetaProvider>(() => metaProviderMock.Object), Enumerable.Repeat<ICoverOrganizer>(target, 1));

			Assert.IsFalse(target.IsCoverExists());
		}

		[Test]
		public void GetCoverFullPath_should_return_cofer_stream()
		{
			var metaProviderMock = new Mock<IMetaProvider>();
			var rootFolder = new RootFolder(PathToFolderWithCover);

			var target = new DirectoryCoverOrganizer();
			var audioFile = new AudioFile("04 - �����.mp3", rootFolder, new Lazy<IMetaProvider>(() => metaProviderMock.Object), Enumerable.Repeat<ICoverOrganizer>(target, 1));

			var cover = target.GetCover();
			Assert.That(cover, Is.Not.Null);
			Assert.That(cover.Name, Is.EqualTo("cover.jpg"));
			Assert.That(cover.CoverStream, Is.Not.Null);
			Assert.That(cover.Length, Is.EqualTo(56395));
			Assert.That(cover.CoverSize.Width, Is.EqualTo(497));
			Assert.That(cover.CoverSize.Height, Is.EqualTo(500));		
		}

		[Test]
		public void GetCoverStream_should_throw_exception()
		{
			var metaProviderMock = new Mock<IMetaProvider>();
			var rootFolder = new RootFolder(PathToFolderWithoutCover);

			var target = new DirectoryCoverOrganizer();
			var audioFile = new AudioFile("04 - �����.mp3", rootFolder, new Lazy<IMetaProvider>(() => metaProviderMock.Object), Enumerable.Repeat<ICoverOrganizer>(target, 1));

			Assert.Throws<InvalidOperationException>(() => target.GetCover());

		}
	}
}