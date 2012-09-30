using System;
using System.IO;
using System.Windows;
using CoverRetriever.AudioInfo;

using NUnit.Framework;

namespace CoverRetriever.Test.AudioInfo
{
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;

    using FluentAssertions;

    using Microsoft.Reactive.Testing;

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
                target.Album.Should().Be("Wild Obsession");
            }
        }

        [Test]
        public void GetArtist_should_retreive_from_file_album_string_in_windows1251()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithWindowsEncoding)))
            {
                target.Album.Should().Be("Ïèðàòñêèé àëüáîì");
            }
        }

        [Test]
        public void GetArtist_should_get_empty_string_from_none_frame_file()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithEmptyFarame)))
            {
                target.Album.Should().BeNull();
            }
        }

        [Test]
        public void GetArtist_should_retreive_from_file_with_both_tags_and_IDv3_is_empty()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ)))
            {
                target.Album.Should().Be("ß ïîëó÷èë ýòó ðîëü");
            }
        }

        [Test]
        public void GetTrackName_should_get_track_name_from_file_name_if_tag_empty()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithEmptyFarame)))
            {
                target.TrackName.Should().Be("EmptyFrameFile");
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
                target.IsEmpty.Should().BeFalse();
            }
        }

        [Test]
        public void Should_throw_error_on_attempt_to_access_when_instance_was_desposed()
        {
            var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ));
            target.Dispose();
            Assert.Throws<ObjectDisposedException>(() => { var album = target.Album; });
        }

        [Test]
        public void Should_indicate_ability_to_read_write_to_file_covers()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ)))
            {
                    target.IsCanProcessed.Should().BeTrue();
            }
        }

        [Test]
        public void Should_return_True_as_cover_exists()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ)))
            {
                target.IsCoverExists().Should().BeTrue();
            }
        }

        [Test]
        public void Should_return_False_as_cover_does_not_exist()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithEmptyFarame)))
            {
                target.IsCoverExists().Should().BeFalse();
            }
        }

        [Test]
        public void Should_get_cover_from_file()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ)))
            {
                Cover cover = target.GetCover();

                cover.Length.Should().Be(113337);
                cover.CoverSize.Should().Be(new Size(598, 600));
                cover.Name.Should().Be("FrontCover.jpg");
                cover.CoverStream.Should().NotBeNull();
            }
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
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(EmptyFrameFileForImageSave)))
            using (var stream = File.OpenRead(PathUtils.BuildFullResourcePath(ImageFileWithÄÄÒ)))
            {
                var cover = new Cover("FrontCover.jpg", new Size(598, 600), 113337, Observable.Return(stream));

                target.SaveCover(cover).Run(mockObservable);
            }

//			long fileSizeAfter = new FileInfo(BuildFullResourcePath(EmptyFrameFileForImageSave)).Length;

            mockObservable.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            mockObservable.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
//			Assert.That(fileSizeBefore, Is.LessThan(fileSizeAfter));
        }

        [Test]
        public void Should_throws_error_disposer_on_attempt_to_save_cover_in_file()
        {
            var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ));
            target.Dispose();

            Assert.Throws<ObjectDisposedException>(() => target.SaveCover(new Cover()));
        }

        [Test]
        public void should_set_Album_in_to_tags_of_file()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ)))
            {
                target.Album = "TestSetAlbum";

                target.Album.Should().Be("TestSetAlbum");
            }
        }

        [Test]
        public void should_set_Artist_in_to_tags_of_file()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ)))
            {
                target.Artist = "TestSetArtist";

                target.Artist.Should().Be("TestSetArtist");
            }
        }

        [Test]
        public void should_set_Yaer_in_to_tags_of_file()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ)))
            {
                target.Year = "2000";

                target.Year.Should().Be("2000");
            }
        }

        [Test]
        public void should_set_TrackName_in_to_tags_of_file()
        {
            using (var target = new Mp3MetaProvider(PathUtils.BuildFullResourcePath(FileWithÄÄÒ)))
            {
                target.TrackName = "TestSetTitle";

                target.TrackName.Should().Be("TestSetTitle");
            }
        }
    }
}
