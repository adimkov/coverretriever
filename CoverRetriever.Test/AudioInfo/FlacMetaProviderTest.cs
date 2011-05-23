
using System;
using CoverRetriever.AudioInfo;
using NUnit.Framework;

namespace CoverRetriever.Test.AudioInfo
{
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
			Assert.Throws<ObjectDisposedException>(() => target.GetAlbum());
		}

		[Test]
		public void Should_get_Album_from_tag()
		{
			using (var target = new FlacMetaProvider(PathUtils.BuildFullResourcePath(Flac1)))
			{
				var album = target.GetAlbum();
				Assert.That(album, Is.EqualTo("Quiet, Live in Atlanta, 1993"));
			}	
		}
		
		[Test]
		public void Should_get_Artist_from_tag()
		{
			using (var target = new FlacMetaProvider(PathUtils.BuildFullResourcePath(Flac1)))
			{
				var artist = target.GetArtist();
				Assert.That(artist, Is.EqualTo("Smashing Pumpkins"));
			}
		}

		[Test]
		public void Should_get_TrackName_from_tag()
		{
			using (var target = new FlacMetaProvider(PathUtils.BuildFullResourcePath(Flac1)))
			{
				var trackName = target.GetTrackName();
				Assert.That(trackName, Is.EqualTo("Earphoria"));
			}
		}

		[Test]
		public void Should_get_Yaer_from_tag()
		{
			using (var target = new FlacMetaProvider(PathUtils.BuildFullResourcePath(Flac1)))
			{
				var year = target.GetYear();
				Assert.That(year, Is.EqualTo("1993"));
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
	}
}