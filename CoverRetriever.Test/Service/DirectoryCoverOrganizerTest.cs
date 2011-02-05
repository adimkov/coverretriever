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
		[Test]
		public void IsCoverExists_should_return_true()
		{
			var metaProviderMock = new Mock<IMetaProvider>();
			var rootFolder = new RootFolder(@"g:\������\���\[1991] - �������\");

			var target = new DirectoryCoverOrganizer();
			var audioFile = new AudioFile("04 - �����.mp3", rootFolder, new Lazy<IMetaProvider>(() => metaProviderMock.Object), Enumerable.Repeat<ICoverOrganizer>(target, 1));

			Assert.IsTrue(target.IsCoverExists());
			Assert.That(target.CoverName, Is.EqualTo("cover.jpg"));
		}

		[Test]
		public void IsCoverExists_should_return_false()
		{
			var metaProviderMock = new Mock<IMetaProvider>();
			var rootFolder = new RootFolder(@"g:\������\���\[2006] - Family CD2\");

			var target = new DirectoryCoverOrganizer();
			var audioFile = new AudioFile("04 - �����.mp3", rootFolder, new Lazy<IMetaProvider>(() => metaProviderMock.Object), Enumerable.Repeat<ICoverOrganizer>(target, 1));

			Assert.IsFalse(target.IsCoverExists());
		}

		[Test]
		public void IsCoverExists_should_return_true_cover_name_not_default()
		{
			var metaProviderMock = new Mock<IMetaProvider>();
			var rootFolder = new RootFolder(@"g:\������\���\[2003] - �����������\");

			var target = new DirectoryCoverOrganizer();
			var audioFile = new AudioFile("04 - �����.mp3", rootFolder, new Lazy<IMetaProvider>(() => metaProviderMock.Object), Enumerable.Repeat<ICoverOrganizer>(target, 1));

			Assert.IsTrue(target.IsCoverExists());
			Assert.That(target.CoverName, Is.EqualTo("����������� 2.jpg"));
		}
	}
}