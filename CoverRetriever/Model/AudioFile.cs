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

    using CoverRetriever.AudioInfo;
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
                return _metaProvider.Value.Artist;
            }
        }

        /// <summary>
        /// Gets album of composition.
        /// </summary>
        public string Album
        {
            get
            {
                return _metaProvider.Value.Album;
            }
        }

        /// <summary>
        /// Gets year of composition.
        /// </summary>
        public string Year
        {
            get
            {
                return _metaProvider.Value.Year;
            }
        }

        /// <summary>
        /// Gets name of composition.
        /// </summary>
        public string TrackName
        {
            get
            {
                return _metaProvider.Value.TrackName;
            }
        }
    }
}