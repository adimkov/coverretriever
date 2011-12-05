// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemItem.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Base class for all file system items
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Model
{
    using System.Diagnostics;
    using System.IO;

    using CoverRetriever.Common.Validation;

    using Microsoft.Practices.Prism.ViewModel;

    /// <summary>
    /// Base class for all file system items.
    /// </summary>
    [DebuggerDisplay("FileName={Name}")]
    public class FileSystemItem : NotificationObject
    {
        /// <summary>
        /// Backing field for Name property.
        /// </summary>
        private string _name;

        /// <summary>
        /// Backing field for Parent property.
        /// </summary>
        private FileSystemItem _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemItem"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public FileSystemItem(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemItem"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parent">The parent.</param>
        public FileSystemItem(string name, FileSystemItem parent)
        {
            Require.NotNull(parent, "parent");

            Name = name;
            Parent = parent;
        }

        /// <summary>
        /// Gets the file system name.
        /// </summary>
        /// <remarks>Name of file system item.</remarks>
        public string Name
        {
            get
            {
                return _name;
            } 

            private set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets the parent of file system item.
        /// </summary>
        public FileSystemItem Parent
        {
            get
            {
                return _parent;
            }
 
            private set
            {
                _parent = value;
                RaisePropertyChanged("Parent");
            }
        }

        /// <summary>
        /// Gets path of the file system item including parent.
        /// </summary>
        /// <param name="fileSystemItem">The file system item.</param>
        /// <returns>File system path.</returns>
        public static string GetBasePath(FileSystemItem fileSystemItem)
        {
            var path = fileSystemItem.Name;
            if (fileSystemItem.Parent != null)
            {
                path = Path.Combine(GetBasePath(fileSystemItem.Parent), path);
            }

            return path;
        }

        /// <summary>
        /// Gets the file system item full path.
        /// </summary>
        /// <returns>Full path of file onto file system.</returns>
        public virtual string GetFileSystemItemFullPath()
        {
            return GetBasePath(this);
        }
    }
}