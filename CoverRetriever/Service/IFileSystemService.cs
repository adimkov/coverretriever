// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileSystemService.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Service to access to the file system
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Service
{
    using System;

    using CoverRetriever.Model;

    /// <summary>
    /// Service to access to the file system.
    /// </summary>
    public interface IFileSystemService
    {
       /// <summary>
        /// Gets children elements of specified folder.
        /// </summary>
        /// <param name="parent">The root folder.</param>
        /// <returns>Observer of filling operation.</returns>
        IObservable<FileSystemItem> GetChildrenForRootFolder(Folder parent);

        /// <summary>
        /// Check for directory existence on client machine.
        /// </summary>
        /// <param name="directoryPath">The full path.</param>
        /// <returns>Result of checking.</returns>
        bool IsDirectoryExists(string directoryPath);
    }
}