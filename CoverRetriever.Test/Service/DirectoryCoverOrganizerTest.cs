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
		private const string PathToFolderWithoutCover = @"g:\Музыка\ДДТ\[2006] - Family CD2\";
		private const string PathToFolderWithCover = @"g:\Музыка\ДДТ\[1991] - Пластун\";

		[Test]
		public void IsCoverExists_should_return_true()
		{
			var metaProviderMock = new Mock<IMetaProvider>();
			var rootFolder = new RootFolder(PathToFolderWithCover);

			var target = new DirectoryCoverOrganizer();
			var audioFile = new AudioFile("04 - Ветры.mp3", rootFolder, new Lazy<IMetaProvider>(() => metaProviderMock.Object), Enumerable.Repeat<ICoverOrganizer>(target, 1));

			Assert.IsTrue(target.IsCoverExists());
			Assert.That(target.CoverName, Is.EqualTo("cover.jpg"));
		}

		[Test]
		public void IsCoverExists_should_return_false()
		{
			var metaProviderMock = new Mock<IMetaProvider>();
			var rootFolder = new RootFolder(PathToFolderWithoutCover);

			var target = new DirectoryCoverOrganizer();
			var audioFile = new AudioFile("04 - Ветры.mp3", rootFolder, new Lazy<IMetaProvider>(() => metaProviderMock.Object), Enumerable.Repeat<ICoverOrganizer>(target, 1));

			Assert.IsFalse(target.IsCoverExists());
		}

		[Test]
		public void GetCoverFullPath_should_return_cofer_stream()
		{
			var metaProviderMock = new Mock<IMetaProvider>();
			var rootFolder = new RootFolder(PathToFolderWithCover);

			var target = new DirectoryCoverOrganizer();
			var audioFile = new AudioFile("04 - Ветры.mp3", rootFolder, new Lazy<IMetaProvider>(() => metaProviderMock.Object), Enumerable.Repeat<ICoverOrganizer>(target, 1));

			Assert.That(target.GetCoverFullPath(), Is.Not.Null);		
		}

		[Test]
		public void GetCoverStream_should_throw_exception()
		{
			var metaProviderMock = new Mock<IMetaProvider>();
			var rootFolder = new RootFolder(PathToFolderWithoutCover);

			var target = new DirectoryCoverOrganizer();
			var audioFile = new AudioFile("04 - Ветры.mp3", rootFolder, new Lazy<IMetaProvider>(() => metaProviderMock.Object), Enumerable.Repeat<ICoverOrganizer>(target, 1));

			Assert.Throws<InvalidOperationException>(() => target.GetCoverFullPath());

		}
	}
}