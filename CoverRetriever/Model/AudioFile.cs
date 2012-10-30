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
        public void SaveFromTagger()
        {
            metaProvider.Value.Save();
        }

        /// <summary>
        /// Copies the tags from.
        /// </summary>
        /// <param name="source">The source.</param>
        public void CopyTagsFrom(IMetaProvider source)
        {
            MetaProvider.CopyFrom(source);
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
