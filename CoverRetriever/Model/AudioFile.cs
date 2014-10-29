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
            get { return directoryCoverOrganizer; }
        }

        /// <summary>
        /// Gets <see cref="ICoverOrganizer"/> for save in Audio frame.
        /// </summary>
        public ICoverOrganizer FrameCover
        {
            get
            {
                return metaProvider.Value as ICoverOrganizer;
            }
        }

        /// <summary>
        /// Gets an audio tag provider.
        /// </summary>
        public IMetaProvider MetaProvider
        {
            get
            {
                return metaProvider.Value;
            }
        }

        /// <summary>
        /// Saves tag from taggerService.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void SaveFromTagger(SaveSettings settings = null)
        {
            MetaProvider.Save(settings);
            EndEditTags();
        }

        /// <summary>
        /// Copies the tags from.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="settings">The settings.</param>
        public void CopyTagsFrom(IMetaProvider source, SaveSettings settings = null)
        {
            MetaProvider.CopyFrom(source, settings);
        }

        /// <summary>
        /// Determines whether the specified other is equals.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool IsEquals(IMetaProvider other) 
        {
            var current = MetaProvider;

            return current.Album == other.Album &&
                current.Artist == other.Artist &&
                current.TrackName == other.TrackName &&
                current.Year == other.Year;
        }

        /// <summary>
        /// Begins the edit tags.
        /// </summary>
        public void BeginEditTags()
        {
            ((EditableObject)MetaProvider).BeginEdit();
        }

        /// <summary>
        /// Ends the edit tags.
        /// </summary>
        public void EndEditTags()
        {
           ((EditableObject)MetaProvider).EndEdit();
        }

        /// <summary>
        /// Cancels the edit tags.
        /// </summary>
        public void CancelEditTags()
        {
            ((EditableObject)MetaProvider).CancelEdit();
        }
    }
}
