
using System;
using CoverRetriever.AudioInfo;
using NUnit.Framework;

namespace CoverRetriever.Test.AudioInfo
{
    using FluentAssertions;

    [TestFixture]
    public class FlacMetaProviderTest
    {
        private const string Flac1 = "1.flac";

        [Test]
        public void Should_throw_error_on_attempt_to_initialize_second_time()
        {
            using (var target = new FlacMetaProvider(PathUtils.BuildFullResourcePath(Flac1)))
            {
                Assert.Throws<MetaProviderException>(() => target.Activate("SomeData"));
            }
        }

        [Test]
        public void Should_throw_error_on_attempt_to_access_when_instance_was_desposed()
        {
            var target = new FlacMetaProvider(PathUtils.BuildFullResourcePath(Flac1));
            target.Dispose();
            Assert.Throws<ObjectDisposedException>(() => { var album = target.Album; });
        }

        [Test]
        public void Should_get_Album_from_tag()
        {
            using (var target = new FlacMetaProvider(PathUtils.BuildFullResourcePath(Flac1)))
            {
                target.Album.Should().Be("Quiet, Live in Atlanta, 1993");
            }	
        }
        
        [Test]
        public void Should_get_Artist_from_tag()
        {
            using (var target = new FlacMetaProvider(PathUtils.BuildFullResourcePath(Flac1)))
            {
                target.Artist.Should().Be("Smashing Pumpkins");
            }
        }

        [Test]
        public void Should_get_TrackName_from_tag()
        {
            using (var target = new FlacMetaProvider(PathUtils.BuildFullResourcePath(Flac1)))
            {
                target.TrackName.Should().Be("Earphoria");
            }
        }

        [Test]
        public void Should_get_Yaer_from_tag()
        {
            using (var target = new FlacMetaProvider(PathUtils.BuildFullResourcePath(Flac1)))
            {
                target.Year.Should().Be("1993");
            }
        }

        [Test]
        public void Should_indicate_that_tag_in_not_empty()
        {
            using (var target = new FlacMetaProvider(PathUtils.BuildFullResourcePath(Flac1)))
            {
                Assert.IsFalse(target.IsEmpty);
            }
        }

        [Test]
        public void should_set_Album_in_to_tags_of_file()
        {
            using (var target = new FlacMetaProvider(PathUtils.BuildFullResourcePath(Flac1)))
            {
                target.Album = "TestSetAlbum";

                target.Album.Should().Be("TestSetAlbum");
            }
        }

        [Test]
        public void should_set_Artist_in_to_tags_of_file()
        {
            using (var target = new FlacMetaProvider(PathUtils.BuildFullResourcePath(Flac1)))
            {
                target.Artist = "TestSetArtist";

                target.Artist.Should().Be("TestSetArtist");
            }
        }

        [Test]
        public void should_set_Yaer_in_to_tags_of_file()
        {
            using (var target = new FlacMetaProvider(PathUtils.BuildFullResourcePath(Flac1)))
            {
                target.Year = "2000";

                target.Year.Should().Be("2000");
            }
        }

        [Test]
        public void should_set_TrackName_in_to_tags_of_file()
        {
            using (var target = new FlacMetaProvider(PathUtils.BuildFullResourcePath(Flac1)))
            {
                target.TrackName = "TestSetTitle";

                target.TrackName.Should().Be("TestSetTitle");
            }
        }
    }
}