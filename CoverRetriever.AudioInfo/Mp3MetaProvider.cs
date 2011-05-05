using System;
using System.ComponentModel.Composition;
using System.IO;
using TagLib;
using File = TagLib.File;

namespace CoverRetriever.AudioInfo
{
	[Export("mp3", typeof(IMetaProvider))]
	[PartCreationPolicy(CreationPolicy.NonShared)]	
	public class Mp3MetaProvider : AudioFileMetaProvider, IDisposable, IActivator
	{
		private File _file;
		private bool _initialized;

		static Mp3MetaProvider()
		{
			TagLib.Id3v1.Tag.DefaultStringHandler = new AutoStringHandler();
			TagLib.Id3v2.Tag.DefaultEncoding = StringType.Latin1;
			ByteVector.UseBrokenLatin1Behavior = true;
		}

		public Mp3MetaProvider()
		{
		}

		public Mp3MetaProvider(string fileName)
		{
			_file = File.Create(fileName, ReadStyle.None);
			ParseFileName(Path.GetFileNameWithoutExtension(fileName));			
			_initialized = true;
		}

		public override string GetAlbum()
		{
			var id3 = GetAudioTag(_file, x => !String.IsNullOrEmpty(x.Album));
			return id3.Album ?? base.GetAlbum();
		}

		public override string GetArtist()
		{
			var id3 = GetAudioTag(_file, x => !String.IsNullOrEmpty(x.FirstArtist));
			return id3.FirstArtist ?? base.GetArtist();
		}

		public override string GetTrackName()
		{
			var id3 = GetAudioTag(_file, x => !String.IsNullOrEmpty(x.Title));
			return id3.Title ?? base.GetTrackName();
		}

		public override string GetYear()
		{
			var id3 = GetAudioTag(_file, x => x.Year > 0);
			return id3.Year > 0 ? id3.Year.ToString() : base.GetYear();
		}

		public void Dispose()
		{
			if (_file != null)
			{
				_file.Dispose();
			}
		}

		public void Activate(params object[] param)
		{
			if (!_initialized)
			{
				var filePath = (string) param[0];
				_file = File.Create(filePath, ReadStyle.None);
				ParseFileName(Path.GetFileNameWithoutExtension(filePath));
				_initialized = true;
			}
			else
			{
				throw new MetaProviderException("Instance already has been initialized");
			}
		}

		/// <summary>
		/// Get tab from file.
		/// <remarks>
		///	IDv2 has highest priority.
		/// </remarks>
		/// </summary>
		/// <param name="file"></param>
		/// <param name="isCorrect">Validator for ñheck tag correct. If tag is invalid, try to get another tag</param>
		/// <returns></returns>
		private Tag GetAudioTag(File file, Func<Tag,bool> isCorrect)
		{
			if ((file.TagTypesOnDisk & TagTypes.Id3v2) == TagTypes.Id3v2 && 
				isCorrect(file.GetTag(TagTypes.Id3v2)))
			{
				return file.GetTag(TagTypes.Id3v2);
			}
			return file.GetTag(TagTypes.Id3v1);
		}
	}
}