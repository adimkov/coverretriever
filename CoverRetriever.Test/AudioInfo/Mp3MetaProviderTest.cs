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
		
		[Test]
		public void GetArtist_should_retreive_from_file_album_string_in_latin()
		{
			var target = new Mp3MetaProvider(BuildFullResourcePate(FileWithLatinEncoding));
			Assert.That(target.GetAlbum(), Is.EqualTo("Wild Obsession"));
		}

		[Test]
		public void GetArtist_should_retreive_from_file_album_string_in_windows1251()
		{
			var target = new Mp3MetaProvider(BuildFullResourcePate(FileWithWindowsEncoding));
			Assert.That(target.GetAlbum(), Is.EqualTo("Пиратский альбом"));
		}

		[Test]
		public void GetArtist_should_get_empty_string_from_none_frame_file()
		{
			var target = new Mp3MetaProvider(BuildFullResourcePate(FileWithEmptyFarame));
			Assert.That(target.GetAlbum(), Is.Null);
		}

		private string BuildFullResourcePate(string fileName)
		{
			string resourceFolder = "Input.AudioInfo";
			return Path.Combine(Environment.CurrentDirectory, resourceFolder, fileName);
		}
	}
}