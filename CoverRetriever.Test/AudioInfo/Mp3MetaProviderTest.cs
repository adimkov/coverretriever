using System;
using System.IO;

using CoverRetriever.AudioInfo;

using NUnit.Framework;

namespace CoverRetriever.Test.AudioInfo
{
	[TestFixture]
	public class Mp3MetaProviderTest
	{
		private const string FileWithLatinEncoding = "LatinEncoding.mp3";
		private const string FileWithWindowsEncoding = "WindowsEncoding.mp3";
		private const string FileWithEmptyFarame = "EmptyFrameFile.mp3";
		private const string FileWithÄÄÒ = "ÄÄÒ - Ïîıò.mp3";
		
		[Test]
		public void GetArtist_should_retreive_from_file_album_string_in_latin()
		{
			using(var target = new Mp3MetaProvider(BuildFullResourcePate(FileWithLatinEncoding)))
			{
				Assert.That(target.GetAlbum(), Is.EqualTo("Wild Obsession"));
			}
		}

		[Test]
		public void GetArtist_should_retreive_from_file_album_string_in_windows1251()
		{
			using (var target = new Mp3MetaProvider(BuildFullResourcePate(FileWithWindowsEncoding)))
			{
				Assert.That(target.GetAlbum(), Is.EqualTo("Ïèğàòñêèé àëüáîì"));
			}
		}

		[Test]
		public void GetArtist_should_get_empty_string_from_none_frame_file()
		{
			using(var target = new Mp3MetaProvider(BuildFullResourcePate(FileWithEmptyFarame)))
			{
				Assert.That(target.GetAlbum(), Is.Null);
			}
		}

		[Test]
		public void GetArtist_should_retreive_from_file_with_both_tags_and_IDv3_is_empty()
		{
			using(var target = new Mp3MetaProvider(BuildFullResourcePate(FileWithÄÄÒ)))
			{
				Assert.That(target.GetAlbum(), Is.EqualTo("ß ïîëó÷èë ıòó ğîëü"));
			}
		}

		[Test]
		public void Should_throw_error_on_attempt_to_initialize_second_time()
		{
			using (var target = new Mp3MetaProvider(BuildFullResourcePate(FileWithÄÄÒ)))
			{
				Assert.Throws<MetaProviderException>(() => target.Activate("SomeData"));
			}		
		}

		[Test]
		public void Should_throw_error_on_attempt_to_access_when_instance_was_desposed()
		{
			var target = new Mp3MetaProvider(BuildFullResourcePate(FileWithÄÄÒ));
			target.Dispose();
			Assert.Throws<ObjectDisposedException>(() => target.GetAlbum());
		}

		private string BuildFullResourcePate(string fileName)
		{
			string resourceFolder = "Input.AudioInfo";
			return Path.Combine(Environment.CurrentDirectory, resourceFolder, fileName);
		}
	}
}