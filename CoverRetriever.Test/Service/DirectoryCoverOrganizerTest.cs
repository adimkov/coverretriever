using System;

using CoverRetriever.Service;


using NUnit.Framework;

namespace CoverRetriever.Test.Service
{
    [TestFixture]
    public class DirectoryCoverOrganizerTest
    {
        private readonly string _pathToFolderWithoutCover;
        private readonly string _pathToFolderWithCover;

        public DirectoryCoverOrganizerTest()
        {
            _pathToFolderWithoutCover = PathUtils.BuildFullResourcePath(@"Empty\");
            _pathToFolderWithCover = PathUtils.BuildFullResourcePath(String.Empty);
        }

        [Test]
        public void IsCoverExists_should_return_true()
        {
            var target = new DirectoryCoverOrganizer(_pathToFolderWithCover);
            
            Assert.IsTrue(target.IsCoverExists());
            Assert.That(target.CoverName, Is.EqualTo("cover.jpg"));
        }

        [Test]
        public void IsCoverExists_should_return_false()
        {
            var target = new DirectoryCoverOrganizer(_pathToFolderWithoutCover);
            
            Assert.IsFalse(target.IsCoverExists());
        }

        [Test]
        public void GetCoverFullPath_should_return_cofer_stream()
        {
            var target = new DirectoryCoverOrganizer(_pathToFolderWithCover);
            
            var cover = target.GetCover();
            Assert.That(cover, Is.Not.Null);
            Assert.That(cover.Name, Is.EqualTo("cover.jpg"));
            Assert.That(cover.CoverStream, Is.Not.Null);
            Assert.That(cover.Length, Is.EqualTo(66183));
            Assert.That(cover.CoverSize.Width, Is.EqualTo(426));
            Assert.That(cover.CoverSize.Height, Is.EqualTo(425));		
        }

        [Test]
        public void GetCoverStream_should_throw_exception()
        {
            var target = new DirectoryCoverOrganizer(_pathToFolderWithoutCover);
            
            Assert.Throws<InvalidOperationException>(() => target.GetCover());
        }

        [Test]
        public void Should_check_for_ability_work_with_cover_in_directory_return_true()
        {
            var target = new DirectoryCoverOrganizer(_pathToFolderWithCover);
            
            Assert.IsTrue(target.IsCanProcessed);
        }
    }
}