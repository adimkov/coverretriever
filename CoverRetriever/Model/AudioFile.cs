// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioFile.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  File represented by Audio composition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Model
{
    using System;
    using System.Linq;

    using CoverRetriever.AudioInfo;
    using CoverRetriever.AudioInfo.Tagger;
    using CoverRetriever.Common.Validation;
    using CoverRetriever.Service;

    /// <summary>
    /// File represented by Audio composition.
    /// </summary>
    /// <remarks>The file participates in file system hierarchy.</remarks>
    public class AudioFile : FileSystemItem
    {
        /// <summary>
        /// Audio tag provider.
        /// </summary>
        private readonly Lazy<IMetaProvider> _metaProvider;

        /// <summary>
        /// Service that saves cover into folder.
        /// </summary>
        private readonly DirectoryCoverOrganizer _directoryCoverOrganizer;

        /// <summary>
        /// The audio file meta data obtainer.
        /// </summary>
        private ITagger _tagger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioFile"/> class.
        /// </summary>
        /// <param name="name">The name of file.</param>
        /// <param name="parent">The parent of file.</param>
        /// <param name="metaProvider">The audio tag provider.</param>
        public AudioFile(string name, FileSystemItem parent, Lazy<IMetaProvider> metaProvider) 
            : base(name, parent)
        {
            Require.NotNull(parent, "parent");
            Require.NotNull(metaProvider, "metaProvider");
            
            _metaProvider = metaProvider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioFile"/> class.
        /// </summary>
        /// <param name="name">The name of file.</param>
        /// <param name="parent">The parent of file.</param>
        /// <param name="metaProvider">The audio tag provider.</param>
        /// <param name="directoryCoverOrganizer">Service that saves cover into folder.</param>
        public AudioFile(string name, FileSystemItem parent, Lazy<IMetaProvider> metaProvider, DirectoryCoverOrganizer directoryCoverOrganizer)
            : this(name, parent, metaProvider)
        {
            _directoryCoverOrganizer = directoryCoverOrganizer;
        }

        /// <summary>
        /// Gets  <see cref="ICoverOrganizer"/> for save in folder.
        /// </summary>
        public ICoverOrganizer DirectoryCover
        {
            get { return _directoryCoverOrganizer; }
        }

        /// <summary>
        /// Gets <see cref="ICoverOrganizer"/> for save in Audio frame.
        /// </summary>
        public ICoverOrganizer FrameCover
        {
            get
            {
                return _metaProvider.Value as ICoverOrganizer;
            }
        }

        /// <summary>
        /// Gets the artist of composition.
        /// </summary>
        public string Artist
        {
            get
            {
                return GetMetaProvider().GetArtist();
            }
        }

        /// <summary>
        /// Gets album of composition.
        /// </summary>
        public string Album
        {
            get
            {
                return GetMetaProvider().GetAlbum();
            }
        }

        /// <summary>
        /// Gets year of composition.
        /// </summary>
        public string Year
        {
            get
            {
                return GetMetaProvider().GetYear();
            }
        }

        /// <summary>
        /// Gets name of composition.
        /// </summary>
        public string TrackName
        {
            get
            {
                return GetMetaProvider().GetTrackName();
            }
        }

        /// <summary>
        /// Gets an audio tag provider.
        /// </summary>
        public IMetaProvider MetaProvider
        {
            get
            {
                return GetMetaProvider();
            }
        }

        /// <summary>
        /// Assigns the tagger.
        /// </summary>
        /// <param name="tagger">The tagger.</param>
        /// <returns>Operation observer.</returns>
        public IObservable<Unit> AssignTagger(ITagger tagger)
        {
            return tagger
                .LoadTagsForAudioFile(GetFileSystemItemFullPath())
                .Do(_ =>
                    {
                        _tagger = tagger;
                        RaisePropertyChanged(String.Empty);
                    });
        }

        /// <summary>
        /// Resets the tagger.
        /// </summary>
        public void ResetTagger()
        {
            _tagger = null;
            RaisePropertyChanged(String.Empty);
        }

        /// <summary>
        /// Saves tag from tagger.
        /// </summary>
        public void SaveFromTagger()
        {
            if (_tagger == null)
            {
                throw new InvalidOperationException("Tagger was not assigned. Assign the tagger first");
            }

//            _tagger.SaveTagsInTo(_metaProvider.Value);
        }

        /// <summary>
        /// Gets the meta provider from file or from tagger.
        /// </summary>
        /// <returns>The meta provider.</returns>
        private IMetaProvider GetMetaProvider()
        {
            if (_tagger != null)
            {
                return _tagger;
            }

            return _metaProvider.Value;
        }

    }
}