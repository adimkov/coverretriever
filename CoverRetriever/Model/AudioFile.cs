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
        private readonly Lazy<IMetaProvider> metaProvider;

        /// <summary>
        /// Service that saves cover into folder.
        /// </summary>
        private readonly DirectoryCoverOrganizer directoryCoverOrganizer;

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
            
            this.metaProvider = metaProvider;
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
            this.directoryCoverOrganizer = directoryCoverOrganizer;
        }

        /// <summary>
        /// Gets  <see cref="ICoverOrganizer"/> for save in folder.
        /// </summary>
        public ICoverOrganizer DirectoryCover
        {
            get { return this.directoryCoverOrganizer; }
        }

        /// <summary>
        /// Gets <see cref="ICoverOrganizer"/> for save in Audio frame.
        /// </summary>
        public ICoverOrganizer FrameCover
        {
            get
            {
                return this.metaProvider.Value as ICoverOrganizer;
            }
        }

        /// <summary>
        /// Gets the artist of composition.
        /// </summary>
        public string Artist
        {
            get
            {
                return MetaProvider.Artist;
            }
        }

        /// <summary>
        /// Gets album of composition.
        /// </summary>
        public string Album
        {
            get
            {
                return MetaProvider.Album;
            }
        }

        /// <summary>
        /// Gets year of composition.
        /// </summary>
        public string Year
        {
            get
            {
                return MetaProvider.Year;
            }
        }

        /// <summary>
        /// Gets name of composition.
        /// </summary>
        public string TrackName
        {
            get
            {
                return MetaProvider.TrackName;
            }
        }

        /// <summary>
        /// Gets an audio tag provider.
        /// </summary>
        public IMetaProvider MetaProvider
        {
            get
            {
                return this.metaProvider.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is needed to retrieve tags.
        /// </summary>
        /// <value>
        ///     <c>true</c> If this instance is needed to retrieve tags; otherwise, <c>false</c>.
        /// </value>
        public bool IsNeededToRetrieveTags
        {
            get
            {
                return String.IsNullOrWhiteSpace(Artist);
            }
        }

        /// <summary>
        /// Resets the taggerService.
        /// </summary>
        public void ResetTagger()
        {
            ((EditableObject)this.metaProvider.Value).CancelEdit();
            RaisePropertyChanged(String.Empty);
        }

        /// <summary>
        /// Saves tag from taggerService.
        /// </summary>
        public void SaveFromTagger()
        {
            ((EditableObject)this.metaProvider.Value).EndEdit();
            metaProvider.Value.Save();
        }

        /// <summary>
        /// Copies the tags from.
        /// </summary>
        /// <param name="source">The source.</param>
        public void CopyTagsFrom(IMetaProvider source)
        {
            MetaProvider.CopyFrom(source);
            RaisePropertyChanged(String.Empty);
        }
    }
}
