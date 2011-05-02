using System;
using System.ComponentModel.Composition;
using TagLib;

namespace CoverRetriever.AudioInfo
{
	[Export("flac", typeof(IMetaProvider))]
	[PartCreationPolicy(CreationPolicy.NonShared)]	
	public class FlacMetaProvider : IMetaProvider, IActivator, IDisposable
	{
		private File _file;
		private bool _initialized;
		private bool _disposed;

		public FlacMetaProvider()
		{
		}

		public FlacMetaProvider(string filePath)
		{
			_file = File.Create(filePath, ReadStyle.None);
			_initialized = true;
		}

		/// <summary>
		/// Indicate is Meta Data empty
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				EnsureInstanceWasNotDisposed();
				return _file.Tag.IsEmpty;
			}
		}

		/// <summary>
		/// Get an album name
		/// </summary>
		/// <returns></returns>
		public string GetAlbum()
		{
			EnsureInstanceWasNotDisposed();
			return _file.Tag.Album;
		}

		/// <summary>
		/// Get an artist
		/// </summary>
		/// <returns></returns>
		public string GetArtist()
		{
			EnsureInstanceWasNotDisposed();
			return _file.Tag.FirstArtist;
		}

		/// <summary>
		/// Get year of album
		/// </summary>
		/// <returns></returns>
		public string GetYear()
		{
			EnsureInstanceWasNotDisposed();
			return _file.Tag.Year.ToString();
		}

		/// <summary>
		/// Get name  of track
		/// </summary>
		/// <returns></returns>
		public string GetTrackName()
		{
			EnsureInstanceWasNotDisposed();
			return _file.Tag.Title;
		}

		/// <summary>
		/// Activate an object
		/// </summary>
		/// <param name="param"></param>
		public void Activate(params object[] param)
		{
			if (!_initialized)
			{
				var filePath = (string) param[0];
				_file = File.Create(filePath, ReadStyle.None);
				_initialized = true;
			}
			else
			{
				throw new MetaProviderException("Instance already has been initialized");
			}
		}

		public void Dispose()
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

		private void EnsureInstanceWasNotDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("Meta provider was disposed");
			}
		}
	}
}