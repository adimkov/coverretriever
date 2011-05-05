using CoverRetriever.AudioInfo;
using NUnit.Framework;

namespace CoverRetriever.Test.AudioInfo
{
	[TestFixture]
	public class FileNameMetaProviderTest
	{
		private const string Pattern1 = "%artist%-%title%";
		private const string Pattern2 = "%title%";
		private const string Pattern3 = "%track%.%artist%-%title%";
		private const string Pattern4 = "%track%.%title%";
		private const string Pattern5 = "%track%-%title%";
		private const string Pattern6 = "%track% %artist%-%title%";
		private const string Pattern7 = "%track% - %title%";
		private const string FileName1 = "UArtist-UTitle";
		private const string FileNameSpaces1 = "UArtist - UTitle";
		private const string FileName2 = "UTitle";
		private const string FileNameSpaces2 = " UTitle ";
		private const string FileName3 = "04.UArtist-UTitle";
		private const string FileNameSpaces3 = "13.UArtist - UTitle";
		private const string FileName4 = "04 UArtist-UTitle";
		private const string FileNameSpaces4 = "04 UArtist - UTitle";
		private const string FileName5 = "04.UTitle";
		private const string FileNameSpaces5 = "04. UTitle";
		private const string FileName6 = "04-UTitle";
		private const string FileNameSpaces6 = "04 - UTitle";
		

		[Test]
		public void Should_obtain_artist_and_title_from_file_name_by_Pattern1()
		{
			var target = new AudioFileMetaProvider(Pattern1);
			target.ParseFileName(FileName1);

			Assert.That("UArtist", Is.EqualTo(target.GetArtist()));
			Assert.That("UTitle", Is.EqualTo(target.GetTrackName()));
		}

		[Test]
		public void Should_obtain_artist_and_title_from_file_name_by_Pattern1_with_Spaces()
		{
			var target = new AudioFileMetaProvider(Pattern1);
			target.ParseFileName(FileNameSpaces1);

			Assert.That("UArtist", Is.EqualTo(target.GetArtist()));
			Assert.That("UTitle", Is.EqualTo(target.GetTrackName()));
		}

		[Test]
		public void Should_obtain_title_from_file_name_by_Pattern12_without_artist()
		{
			var target = new AudioFileMetaProvider(Pattern1, Pattern2);
			target.ParseFileName(FileName2);

			Assert.That("UTitle", Is.EqualTo(target.GetTrackName()));
		}

		[Test]
		public void Should_obtain_title_from_file_name_by_Pattern12_without_artist_with_spaces()
		{
			var target = new AudioFileMetaProvider(Pattern1, Pattern2);
			target.ParseFileName(FileNameSpaces2);

			Assert.That("UTitle", Is.EqualTo(target.GetTrackName()));
		}

		[Test]
		public void Should_obtain_artist_title_from_file_name_by_Pattern123_without_artist()
		{
			var target = new AudioFileMetaProvider(Pattern1, Pattern2, Pattern3);
			target.ParseFileName(FileName3);

			Assert.That("UArtist", Is.EqualTo(target.GetArtist()));
			Assert.That("UTitle", Is.EqualTo(target.GetTrackName()));
		}
		
		[Test]
		public void Should_obtain_artist_title_from_file_name_by_Pattern123_without_artist_with_spaces()
		{
			var target = new AudioFileMetaProvider(Pattern1, Pattern2, Pattern3);
			target.ParseFileName(FileNameSpaces3);

			Assert.That("UArtist", Is.EqualTo(target.GetArtist()));
			Assert.That("UTitle", Is.EqualTo(target.GetTrackName()));
		}

		[Test]
		public void Should_obtain_artist_title_from_file_name_by_Pattern1236()
		{
			var target = new AudioFileMetaProvider(Pattern1, Pattern2, Pattern3, Pattern6);
			target.ParseFileName(FileName4);

			Assert.That("UArtist", Is.EqualTo(target.GetArtist()));
			Assert.That("UTitle", Is.EqualTo(target.GetTrackName()));
		}

		[Test]
		public void Should_obtain_artist_title_from_file_name_by_Pattern1236_spaces()
		{
			var target = new AudioFileMetaProvider(Pattern1, Pattern2, Pattern3, Pattern6);
			target.ParseFileName(FileNameSpaces4);

			Assert.That("UArtist", Is.EqualTo(target.GetArtist()));
			Assert.That("UTitle", Is.EqualTo(target.GetTrackName()));
		}

		[Test]
		public void Should_obtain_artist_title_from_file_name_by_Pattern1234_for_FileName5()
		{
			var target = new AudioFileMetaProvider(Pattern1, Pattern2, Pattern3, Pattern4);
			target.ParseFileName(FileName5);

			Assert.That("UTitle", Is.EqualTo(target.GetTrackName()));
		}

		[Test]
		public void Should_obtain_artist_title_from_file_name_by_Pattern1234_for_FileNameSpaces5()
		{
			var target = new AudioFileMetaProvider(Pattern1, Pattern2, Pattern3, Pattern4);
			target.ParseFileName(FileNameSpaces5);

			Assert.That("UTitle", Is.EqualTo(target.GetTrackName()));
		}

		[Test]
		public void Should_obtain_artist_title_from_file_name_by_Pattern12345_for_FileName6()
		{
			var target = new AudioFileMetaProvider(Pattern1, Pattern2, Pattern3, Pattern4, Pattern5);
			target.ParseFileName(FileName6);

			Assert.That("UTitle", Is.EqualTo(target.GetTrackName()));
		}

		[Test]
		public void Should_obtain_artist_title_from_file_name_by_Pattern123457_for_FileNameSpaces6()
		{
			var target = new AudioFileMetaProvider(Pattern1, Pattern2, Pattern3, Pattern4, Pattern5, Pattern7);
			target.ParseFileName(FileNameSpaces6);

			Assert.That("UTitle", Is.EqualTo(target.GetTrackName()));
		}
	}
}