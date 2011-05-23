
using System;
using System.IO;
using System.Linq;
using CoverRetriever.AudioInfo.Helper;
using TagLib;
using File = TagLib.File;

namespace CoverRetriever.AudioInfo
{
	/// <summary>
	/// Obtain Meta info from file name
	/// </summary>
	public abstract class AudioFileMetaProvider : IMetaProvider, IActivator, ICoverOrganizer, IDisposable
	{
		private File _file;
		private bool _initialized;
		private bool _disposed;

		public AudioFileMetaProvider()
		{
			FileNameMetaObtainer = new FileNameMetaObtainer();
		}

		/// <summary>
		/// Indicate is Meta Data empty
		/// </summary>
		public virtual bool IsEmpty
		{
			get
			{
				EnsureInstanceWasNotDisposed();
				return _file.Tag.IsEmpty;
			}
		}

		/// <summary>
		/// Indicate if can work with cover
		/// </summary>
		public virtual bool IsCanProcessed
		{
			get
			{
				EnsureInstanceWasNotDisposed();
				return true;
			}
		}

		/// <summary>
		/// Get FileNameMetaObtainer 
		/// </summary>
		protected FileNameMetaObtainer FileNameMetaObtainer { get; private set; }

		/// <summary>
		/// Get tag file
		/// </summary>
		protected File File
		{
			get { return _file; }
		}

		/// <summary>
		/// Get an album name
		/// </summary>
		/// <returns></returns>
		public virtual string GetAlbum()
		{
			EnsureInstanceWasNotDisposed();
			return _file.Tag.Album ?? FileNameMetaObtainer.GetAlbum();
		}

		/// <summary>
		/// Get an artist
		/// </summary>
		/// <returns></returns>
		public virtual string GetArtist()
		{
			EnsureInstanceWasNotDisposed();
			return _file.Tag.FirstArtist ?? FileNameMetaObtainer.GetArtist();
		}

		/// <summary>
		/// Get year of album
		/// </summary>
		/// <returns></returns>
		public virtual string GetYear()
		{
			EnsureInstanceWasNotDisposed();
			return _file.Tag.Year >= 0 ? _file.Tag.Year.ToString() : FileNameMetaObtainer.GetYear();
		}

		/// <summary>
		/// Get name  of track
		/// </summary>
		/// <returns></returns>
		public virtual string GetTrackName()
		{
			EnsureInstanceWasNotDisposed();
			return _file.Tag.Title ?? FileNameMetaObtainer.GetTrackName();
		}

		
		/// <summary>
		/// Indicate the cover existence
		/// </summary>
		/// <returns><see cref="True"/> if cover exists</returns>
		public virtual bool IsCoverExists()
		{
			EnsureInstanceWasNotDisposed();
			return _file.Tag.GetCoverSafe(PictureType.FrontCover) != null;
		}

		/// <summary>
		/// Get cover
		/// </summary>
		/// <returns>Cover info</returns>
		public virtual Cover GetCover()
		{
			EnsureInstanceWasNotDisposed();
			return _file.Tag.GetCoverSafe(PictureType.FrontCover).PrepareCover();
		}

		/// <summary>
		/// Save stream into cover
		/// </summary>
		/// <param name="cover">Cover to save</param>
		public virtual IObservable<Unit> SaveCover(Cover cover)
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

		/// <summary>
		/// Activate an object
		/// </summary>
		/// <param name="param"></param>
		public void Activate(params object[] param)
		{
			if (!_initialized)
			{
				var filePath = (string)param[0];
				_file = GetTagFile(filePath);
				FileNameMetaObtainer.ParseFileName(Path.GetFileNameWithoutExtension(filePath));
				_initialized = true;
			}
			else
			{
				throw new MetaProviderException("Instance already has been initialized");
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public virtual void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
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

		protected virtual File GetTagFile(string fileName)
		{
			return File.Create(fileName, ReadStyle.None);
		}

		protected void EnsureInstanceWasNotDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("Meta provider was disposed");
			}
		}
	}
}