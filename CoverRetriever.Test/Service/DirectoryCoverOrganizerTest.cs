using System;

using CoverRetriever.Service;


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
			var target = new DirectoryCoverOrganizer(PathToFolderWithCover);
			
			Assert.IsTrue(target.IsCoverExists());
			Assert.That(target.CoverName, Is.EqualTo("cover.jpg"));
		}

		[Test]
		public void IsCoverExists_should_return_false()
		{
			var target = new DirectoryCoverOrganizer(PathToFolderWithoutCover);
			
			Assert.IsFalse(target.IsCoverExists());
		}

		[Test]
		public void GetCoverFullPath_should_return_cofer_stream()
		{
			var target = new DirectoryCoverOrganizer(PathToFolderWithCover);
			
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
			var target = new DirectoryCoverOrganizer(PathToFolderWithoutCover);
			
			Assert.Throws<InvalidOperationException>(() => target.GetCover());
		}
	}
}