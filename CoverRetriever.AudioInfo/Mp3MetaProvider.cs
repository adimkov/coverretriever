using System.ComponentModel.Composition;

using TagLib;

namespace CoverRetriever.AudioInfo
{
	[Export(typeof(IMetaProvider))]
	public class Mp3MetaProvider : IMetaProvider
	{
		private readonly File _file;
		
		static Mp3MetaProvider()
		{
			TagLib.Id3v1.Tag.DefaultStringHandler = new AutoStringHandler();
			TagLib.Id3v2.Tag.DefaultEncoding = StringType.Latin1;
			ByteVector.UseBrokenLatin1Behavior = true;	
		}
		
		public Mp3MetaProvider(string fileName)
		{
			_file = File.Create(fileName, ReadStyle.None);
		}

		public string GetAlbum()
		{
			var id3 = GetPrioritizedTag(_file);
			return id3.Album;
		}

		public string GetArtist()
		{
			var id3 = GetPrioritizedTag(_file);
			return id3.FirstArtist;
		}

		public string GetTrackName()
		{
			var id3 = GetPrioritizedTag(_file);
			return id3.Title;
		}

		public string GetYear()
		{
			var id3 = GetPrioritizedTag(_file);
			return id3.Year.ToString();
		}

		/// <summary>
		/// Get tab from file.
		/// <remarks>
		///	IDv2 has highest priority
		/// </remarks>
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		private Tag GetPrioritizedTag(File file)
		{
			if ((file.TagTypesOnDisk & TagTypes.Id3v2) == TagTypes.Id3v2)
			{
				return file.GetTag(TagTypes.Id3v2);
			}
			return file.GetTag(TagTypes.Id3v1);
		}
	}
}