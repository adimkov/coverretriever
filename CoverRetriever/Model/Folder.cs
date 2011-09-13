// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Folder.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  A folder contains another file system items.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Model
{
    using System.Collections.ObjectModel;
    using System.IO;

    /// <summary>
    /// A folder contains another file system items.
    /// </summary>
    public class Folder : FileSystemItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Folder"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parent">The parent.</param>
        public Folder(string name, FileSystemItem parent) 
            : base(name, parent)
        {
            Children = new ObservableCollection<FileSystemItem>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Folder"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        protected Folder(string name)
            : base(name)
        {
            Children = new ObservableCollection<FileSystemItem>();
        }

        /// <summary>
        /// Gets the content of folder.
        /// </summary>
        public ObservableCollection<FileSystemItem> Children { get; private set; }

        /// <summary>
        /// Get file system item full path.
        /// </summary>
        /// <returns>Path onto file system.</returns>
        public override string GetFileSystemItemFullPath()
        {
            return base.GetFileSystemItemFullPath() + Path.DirectorySeparatorChar;
        }
    }
}