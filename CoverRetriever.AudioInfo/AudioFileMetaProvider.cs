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
    using System.Reactive;
    using System.Reactive.Linq;

    using CoverRetriever.AudioInfo.Helper;

    using TagLib;

    using File = TagLib.File;

    /// <summary>
    /// Base class to obtain Meta info from file name.
    /// </summary>
    public abstract class AudioFileMetaProvider : EditableObject, IMetaProvider, IActivator, ICoverOrganizer, IDisposable
    {
        /// <summary>
        /// Audio file.
        /// </summary>
        private File file;

        /// <summary>
        /// The flag indicating is audio file initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// The flag indicating is audio file disposed.
        /// </summary>
        private bool disposed;

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
                return file.Tag.IsEmpty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this file is changed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this file is changed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirty
        {
            get
            {
                return IsChanged();
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
        /// Gets or sets an album name.
        /// </summary>
        public virtual string Album
        {
            get
            {
                EnsureInstanceWasNotDisposed();
                return file.Tag.Album ?? FileNameMetaObtainer.GetAlbum();
            }

            set
            {
                EnsureInstanceWasNotDisposed();
                file.Tag.Album = value;
            }
        }

        /// <summary>
        /// Gets or sets an artist.
        /// </summary>
        public virtual string Artist
        {
            get
            {
                EnsureInstanceWasNotDisposed();
                return file.Tag.FirstPerformer ?? FileNameMetaObtainer.GetArtist();
            }

            set
            {
                EnsureInstanceWasNotDisposed();
                file.Tag.Performers = new[] { value };
            }
        }

        /// <summary>
        /// Gets or sets year of album.
        /// </summary>
        public virtual string Year
        {
            get
            {
                EnsureInstanceWasNotDisposed();
                return file.Tag.Year > 0 ? file.Tag.Year.ToString() : FileNameMetaObtainer.GetYear();
            }

            set
            {
                EnsureInstanceWasNotDisposed();
                file.Tag.Year = uint.Parse(value.Return("0"));
            }
        }

        /// <summary>
        /// Gets or sets name of track.
        /// </summary>
        public virtual string TrackName
        {
            get
            {
                EnsureInstanceWasNotDisposed();
                return file.Tag.Title ?? FileNameMetaObtainer.GetTrackName();
            }

            set
            {
                EnsureInstanceWasNotDisposed();
                file.Tag.Title = value;
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
            get { return file; }
        }

        /// <summary>
        /// Saves tags into file instance.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public virtual void Save(SaveSettings settings = null)
        {
            if (settings != null)
            {
                if (!settings.TrackName)
                {
                    RevertProperty("TrackName");
                }

                if (!settings.Album)
                {
                    RevertProperty("Album");
                }

                if (!settings.Artist)
                {
                    RevertProperty("Artist");
                }

                if (!settings.Year)
                {
                    RevertProperty("Year");
                }
            }

            File.Save();
        }

        /// <summary>
        /// Copies metadata from specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="settings">The settings.</param>
        public void CopyFrom(IMetaProvider source, SaveSettings settings = null)
        {
            BeginEdit();
            if (settings == null)
            {
                Album = source.Album;
                Artist = source.Artist;
                Year = source.Year;
                TrackName = source.TrackName;    
            }
            else
            {
                if (settings.Album)
                {
                    Album = source.Album;
                }

                if (settings.Artist)
                {
                    Artist = source.Artist;
                }

                if (settings.Year)
                {
                    Year = source.Year;
                }

                if (settings.TrackName)
                {
                    TrackName = source.TrackName;
                }
            }                                 
        }

        /// <summary>
        /// Gets a value indicating whether the cover existence.
        /// </summary>
        /// <returns><c>true</c> if cover exists; otherwise <c>false</c></returns>
        public virtual bool IsCoverExists()
        {
            EnsureInstanceWasNotDisposed();
            var frontCover = file.Tag.GetCoverSafe(PictureType.FrontCover);
            return frontCover != null && frontCover.IsImageValid();
        }

        /// <summary>
        /// Gets the <see cref="Cover"/> from Tag.
        /// </summary>
        /// <returns>The <see cref="Cover"/>.</returns>
        public virtual Cover GetCover()
        {
            EnsureInstanceWasNotDisposed();
            return file.Tag.GetCoverSafe(PictureType.FrontCover).PrepareCover();
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
                        file.Tag.ReplacePictures(PictureHelper.PreparePicture(stream, cover.Name, PictureType.FrontCover));
                        file.Save();
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
            if (!initialized)
            {
                var filePath = (string)param[0];
                file = GetTagFile(filePath);
                if (IsEmpty)
                {
                    FileNameMetaObtainer.ParseFileName(Path.GetFileNameWithoutExtension(filePath));
                }

                initialized = true;
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
        /// Equals the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>True if tags are equals.</returns>
        public bool Equals(IMetaProvider other)
        {
            return Album == other.Album &&
                Artist == other.Artist &&
                Year == other.Year &&
                TrackName == other.TrackName;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "TrackName: {0}, Artist: {1}, Album {2}, Year: {3}"
                .FormatString(
                TrackName, 
                Artist, 
                Album, 
                Year);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (file != null)
                    {
                        file.Dispose();
                    }
                }

                file = null;
                disposed = true;
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
            if (disposed)
            {
                throw new ObjectDisposedException("Meta provider was disposed");
            }
        }
    }
}