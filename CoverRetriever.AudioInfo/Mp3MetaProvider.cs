using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using CoverRetriever.AudioInfo.Helper;
using TagLib;
using File = TagLib.File;


namespace CoverRetriever.AudioInfo
{
	[Export("mp3", typeof(IMetaProvider))]
	[PartCreationPolicy(CreationPolicy.NonShared)]	
	public class Mp3MetaProvider : AudioFileMetaProvider, IActivator, ICoverOrganizer, IDisposable
	{
		private File _file;
		private bool _initialized;
		private bool _disposed;

		static Mp3MetaProvider()
		{
			TagLib.Id3v1.Tag.DefaultStringHandler = new AutoStringHandler();
			TagLib.Id3v2.Tag.DefaultEncoding = StringType.Latin1;
			ByteVector.UseBrokenLatin1Behavior = true;
		}

		[Obsolete("Added for MEF compatibility")]
		public Mp3MetaProvider()
		{
		}

		public Mp3MetaProvider(string fileName)
		{
			EnsureInstanceWasNotDisposed();
			_file = File.Create(fileName, ReadStyle.None);
			ParseFileName(Path.GetFileNameWithoutExtension(fileName));			
			_initialized = true;
		}

		/// <summary>
		/// Indicate is Meta Data empty
		/// </summary>
		public override bool IsEmpty
		{
			get
			{
				EnsureInstanceWasNotDisposed();
				return _file.GetTag(TagTypes.Id3v2).IsEmpty &&
					_file.GetTag(TagTypes.Id3v1).IsEmpty;
			}
		}

		/// <summary>
		/// Get an album name
		/// </summary>
		/// <returns></returns>
		public override string GetAlbum()
		{
			EnsureInstanceWasNotDisposed();
			var id3 = GetAudioTag(_file, x => !String.IsNullOrEmpty(x.Album));
			return id3.Album ?? base.GetAlbum();
		}

		/// <summary>
		/// Get an artist
		/// </summary>
		/// <returns></returns>
		public override string GetArtist()
		{
			EnsureInstanceWasNotDisposed();
			var id3 = GetAudioTag(_file, x => !String.IsNullOrEmpty(x.FirstArtist));
			return id3.FirstArtist ?? base.GetArtist();
		}

		/// <summary>
		/// Get name  of track
		/// </summary>
		/// <returns></returns>
		public override string GetTrackName()
		{
			EnsureInstanceWasNotDisposed();
			var id3 = GetAudioTag(_file, x => !String.IsNullOrEmpty(x.Title));
			return id3.Title ?? base.GetTrackName();
		}

		/// <summary>
		/// Get year of album
		/// </summary>
		/// <returns></returns>
		public override string GetYear()
		{
			EnsureInstanceWasNotDisposed();
			var id3 = GetAudioTag(_file, x => x.Year > 0);
			return id3.Year > 0 ? id3.Year.ToString() : base.GetYear();
		}

		/// <summary>
		/// Is can save into file
		/// </summary>
		public bool IsCanProcessed
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Indicate the cover existence
		/// </summary>
		/// <returns><see cref="True"/> if cover exists</returns>
		public bool IsCoverExists()
		{
			EnsureInstanceWasNotDisposed();
			return _file.Tag.GetCoverSafe(PictureType.FrontCover) != null;
		}

		/// <summary>
		/// Get cover
		/// </summary>
		/// <returns>Cover info</returns>
		public Cover GetCover()
		{
			EnsureInstanceWasNotDisposed();
			return _file.Tag.GetCoverSafe(PictureType.FrontCover).PrepareCover();
		}

		/// <summary>
		/// Save stream into cover
		/// </summary>
		/// <param name="cover">Cover to save</param>
		public IObservable<Unit> SaveCover(Cover cover)
		{
			EnsureInstanceWasNotDisposed();
			var saveResult = cover.CoverStream
				.Do(
					stream =>
					{
						_file.Tag.ReplacePictures(PictureHelper.PreparePicture(stream, cover.Name, PictureType.FrontCover));
						_file.Save();
					})
				.Select(x => new Unit())
				.Take(1);
			
			return saveResult;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
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

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (_file != null)
					{
						_file.Dispose();
					}
				}
				_file = null;
				_disposed = true;
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

		private void EnsureInstanceWasNotDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("Meta provider was disposed"); 
			}
		}
	}
}