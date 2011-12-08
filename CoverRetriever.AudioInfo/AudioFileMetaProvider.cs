// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioFileMetaProvider.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Base class to obtain Meta info from file name.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo
{
    using System;
    using System.IO;
    using System.Linq;

    using CoverRetriever.AudioInfo.Helper;

    using TagLib;

    using File = TagLib.File;

    /// <summary>
    /// Base class to obtain Meta info from file name.
    /// </summary>
    public abstract class AudioFileMetaProvider : IMetaProvider, IActivator, ICoverOrganizer, IDisposable
    {
        /// <summary>
        /// Audio file.
        /// </summary>
        private File _file;

        /// <summary>
        /// The flag indicating is audio file initialized.
        /// </summary>
        private bool _initialized;

        /// <summary>
        /// The flag indicating is audio file disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioFileMetaProvider"/> class.
        /// </summary>
        protected AudioFileMetaProvider()
        {
            FileNameMetaObtainer = new FileNameMetaObtainer();
        }

        /// <summary>
        /// Gets a value indicating whether Tag empty.
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
        /// Gets a value indicating whether can perform operations with Cover.
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
        /// Gets an album name.
        /// </summary>
        public virtual string Album
        {
            get
            {
                EnsureInstanceWasNotDisposed();
                return _file.Tag.Album ?? FileNameMetaObtainer.GetAlbum();
            }
        }

        /// <summary>
        /// Gets an artist.
        /// </summary>
        public virtual string Artist
        {
            get
            {
                EnsureInstanceWasNotDisposed();
                return _file.Tag.FirstPerformer ?? FileNameMetaObtainer.GetArtist();
            }
        }

        /// <summary>
        /// Gets year of album.
        /// </summary>
        public virtual string Year
        {
            get
            {
                EnsureInstanceWasNotDisposed();
                return _file.Tag.Year >= 0 ? _file.Tag.Year.ToString() : FileNameMetaObtainer.GetYear();
            }
        }

        /// <summary>
        /// Gets name of track.
        /// </summary>
        public virtual string TrackName
        {
            get
            {
                EnsureInstanceWasNotDisposed();
                return _file.Tag.Title ?? FileNameMetaObtainer.GetTrackName();
            }
        }

        /// <summary>
        /// Gets service to obtain name of file. 
        /// </summary>
        protected FileNameMetaObtainer FileNameMetaObtainer { get; private set; }

        /// <summary>
        /// Gets audio file.
        /// </summary>
        protected File File
        {
            get { return _file; }
        }

        /// <summary>
        /// Gets a value indicating whether the cover existence.
        /// </summary>
        /// <returns><c>true</c> if cover exists; otherwise <c>false</c></returns>
        public virtual bool IsCoverExists()
        {
            EnsureInstanceWasNotDisposed();
            return _file.Tag.GetCoverSafe(PictureType.FrontCover) != null;
        }

        /// <summary>
        /// Gets the <see cref="Cover"/> from Tag.
        /// </summary>
        /// <returns>The <see cref="Cover"/>.</returns>
        public virtual Cover GetCover()
        {
            EnsureInstanceWasNotDisposed();
            return _file.Tag.GetCoverSafe(PictureType.FrontCover).PrepareCover();
        }

        /// <summary>
        /// Saves <see cref="Cover"/> in to Tag.
        /// </summary>
        /// <param name="cover"><see cref="Cover"/> to save.</param>
        /// <returns>Operation result observer.</returns>
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
        /// Activates an audio file.
        /// </summary>
        /// <param name="param">Parameters to activate audio file.</param>
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

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
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
        /// Gets the tag file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The <see cref="File"/>.</returns>
        protected virtual File GetTagFile(string fileName)
        {
            return File.Create(fileName, ReadStyle.None);
        }

        /// <summary>
        /// Ensures the instance was not disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">If file was disposed.</exception>
        protected void EnsureInstanceWasNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Meta provider was disposed");
            }
        }
    }
}