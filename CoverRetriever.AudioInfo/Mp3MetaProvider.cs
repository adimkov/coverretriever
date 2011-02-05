using System;
using System.ComponentModel.Composition;


using TagLib;

namespace CoverRetriever.AudioInfo
{
	[Export(typeof(IMetaProvider))]
	public class Mp3MetaProvider : IMetaProvider
	{
		
		File _file;
		public Mp3MetaProvider(string fileName)
		{
			_file = File.Create(fileName, ReadStyle.None);
			TagLib.Id3v1.Tag.DefaultStringHandler = new AutoStringHandler();
			TagLib.Id3v2.Tag.DefaultEncoding = StringType.Latin1;
		}

		public string GetAlbum()
		{
			string album = string.Empty;
			var id3v2 = _file.GetTag(TagTypes.Id3v2);

			if (!String.IsNullOrEmpty(id3v2.Album))
			{
				album = id3v2.Album;
			}
			else
			{
				album = _file.GetTag(TagTypes.Id3v1).Album;
			}
			return album;
		}

		public string GetArtist()
		{
			string artistName = string.Empty;
			var id3v2 = _file.GetTag(TagTypes.Id3v2);

			if (!String.IsNullOrEmpty(id3v2.FirstArtist))
			{
				artistName = id3v2.FirstArtist;
			}
			else
			{
				artistName = _file.GetTag(TagTypes.Id3v1).FirstArtist;
			}

			return artistName;
		}

		public string GetTrackName()
		{
			string trackName = string.Empty;
			var id3v2 = _file.GetTag(TagTypes.Id3v2);
			
			if (!String.IsNullOrEmpty(id3v2.Title))
			{
				trackName = id3v2.Title;
			}
			else
			{
				trackName = _file.GetTag(TagTypes.Id3v1).Title;
			}
			return trackName;
		}

		public string GetYear()
		{
			string year = string.Empty;
			var id3v2 = _file.GetTag(TagTypes.Id3v2);

			if (id3v2.Year > 0)
			{
				year = id3v2.Year.ToString();
			}
			else
			{
				
				year = _file.GetTag(TagTypes.Id3v1).Year.ToString();
			}
			
			return year;
		}
	}
}