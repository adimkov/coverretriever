using System;
using System.Collections.Generic;
using System.Concurrency;
using System.IO;
using System.Reactive.Testing.Mocks;
using System.Windows;
using CoverRetriever.AudioInfo;

using NUnit.Framework;

namespace CoverRetriever.Test.AudioInfo
{
    using System.Linq;

    [TestFixture]
    public class Mp3MetaProviderTest
    {
        private const string FileWithLatinEncoding = "LatinEncoding.mp3";
        private const string FileWithWindowsEncoding = "WindowsEncoding.mp3";
        private const string FileWithEmptyFarame = "EmptyFrameFile.mp3";
        private const string EmptyFrameFileForImageSave = "EmptyFrameFileForImageSave.mp3";
        private const string FileWithÄÄÒ = "DDT-Poet.mp3";
        private const string ImageFileWithÄÄÒ = "DDT-Poet.mp3.jpg";
        
        [Test]
        public void GetArtist_should_retreive_from_file_album_string_in_latin()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithLatinEncoding)))
            {
                Assert.That(target.GetAlbum(), Is.EqualTo("Wild Obsession"));
            }
        }

        [Test]
        public void GetArtist_should_retreive_from_file_album_string_in_windows1251()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithWindowsEncoding)))
            {
                Assert.That(target.GetAlbum(), Is.EqualTo("Ïèðàòñêèé àëüáîì"));
            }
        }

        [Test]
        public void GetArtist_should_get_empty_string_from_none_frame_file()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithEmptyFarame)))
            {
                Assert.That(target.GetAlbum(), Is.Null);
            }
        }

        [Test]
        public void GetArtist_should_retreive_from_file_with_both_tags_and_IDv3_is_empty()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ)))
            {
                Assert.That(target.GetAlbum(), Is.EqualTo("ß ïîëó÷èë ýòó ðîëü"));
            }
        }

        [Test]
        public void GetTrackName_should_get_track_name_from_file_name_if_tag_empty()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithEmptyFarame)))
            {
                Assert.That(target.GetTrackName(), Is.EqualTo("EmptyFrameFile"));
            }	
        }
        [Test]
        public void Should_throw_error_on_attempt_to_initialize_second_time()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ)))
            {
                Assert.Throws<MetaProviderException>(() => target.Activate("SomeData"));
            }		
        }

        [Test]
        public void Should_indicate_that_tag_in_not_empty()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ)))
            {
                Assert.IsFalse(target.IsEmpty);
            }
        }

        [Test]
        public void Should_throw_error_on_attempt_to_access_when_instance_was_desposed()
        {
            var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ));
            target.Dispose();
            Assert.Throws<ObjectDisposedException>(() => target.GetAlbum());
        }

        [Test]
        public void Should_indicate_ability_to_read_write_to_file_covers()
        {
            var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ));
            Assert.IsTrue(target.IsCanProcessed);
        }

        [Test]
        public void Should_return_True_as_cover_exists()
        {
            var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ));
            Assert.IsTrue(target.IsCoverExists());
        }

        [Test]
        public void Should_return_False_as_cover_does_not_exist()
        {
            var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithEmptyFarame));
            Assert.IsFalse(target.IsCoverExists());
        }

        [Test]
        public void Should_get_cover_from_file()
        {
            var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ));
            Cover cover = target.GetCover();

            Assert.That(cover.Length, Is.EqualTo(113337));
            Assert.That(cover.CoverSize, Is.EqualTo(new Size(598, 600)));
            Assert.That(cover.Name, Is.EqualTo("FrontCover.jpg"));
            Assert.That(cover.CoverStream, Is.Not.Null);
        }

        [Test]
        public void Should_throws_error_disposer_on_attempt_to_get_cover_from_file()
        {
            var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ));	
            target.Dispose();

            Assert.Throws<ObjectDisposedException>(() => target.GetCover());
        }

        [Test]
        public void Should_save_cover_in_file()
        {
//			long fileSizeBefore = new FileInfo(BuildFullResourcePath(EmptyFrameFileForImageSave)).Length;
            
            var testScheduler = new TestScheduler();
            var mockObservable = new MockObserver<Unit>(testScheduler);
            var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(EmptyFrameFileForImageSave));
            using (var stream = File.OpenRead(PathUtils.BuildFullResourcePath(ImageFileWithÄÄÒ)))
            {
                var cover = new Cover("FrontCover.jpg", new Size(598, 600), 113337, Observable.Return(stream));

                target.SaveCover(cover).Run(mockObservable);
            }

//			long fileSizeAfter = new FileInfo(BuildFullResourcePath(EmptyFrameFileForImageSave)).Length;
            
            Assert.That(mockObservable[0].Value.Kind, Is.EqualTo(NotificationKind.OnNext));
            Assert.That(mockObservable[1].Value.Kind, Is.EqualTo(NotificationKind.OnCompleted));
//			Assert.That(fileSizeBefore, Is.LessThan(fileSizeAfter));
        }


        [Test]
        public void Should_throws_error_disposer_on_attempt_to_save_cover_in_file()
        {
            var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ));
            target.Dispose();

            Assert.Throws<ObjectDisposedException>(() => target.SaveCover(new Cover()));
        }
    }
}
