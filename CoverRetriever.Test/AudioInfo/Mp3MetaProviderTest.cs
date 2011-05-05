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
		private const string FileWithДДТ = "ДДТ - Поэт.mp3";
		
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
				Assert.That(target.GetAlbum(), Is.EqualTo("Пиратский альбом"));
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
			using(var target = new Mp3MetaProvider(BuildFullResourcePate(FileWithДДТ)))
			{
				Assert.That(target.GetAlbum(), Is.EqualTo("Я получил эту роль"));
			}
		}

		[Test]
		public void GetTrackName_should_get_track_name_from_file_name_if_tag_empty()
		{
			using (var target = new Mp3MetaProvider(BuildFullResourcePate(FileWithEmptyFarame)))
			{
				Assert.That(target.GetTrackName(), Is.EqualTo("EmptyFrameFile"));
			}	
		}

		private string BuildFullResourcePate(string fileName)
		{
			string resourceFolder = "Input.AudioInfo";
			return Path.Combine(Environment.CurrentDirectory, resourceFolder, fileName);
		}
	}
}