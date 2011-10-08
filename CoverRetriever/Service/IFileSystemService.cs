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
    using System.Windows.Threading;

    using CoverRetriever.Model;

    /// <summary>
    /// Service to access to the file system.
    /// </summary>
    public interface IFileSystemService
    {
       /// <summary>
        /// Fills the root folder with subfolders and audio files async.
        /// </summary>
        /// <param name="parent">The root folder.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <returns>Observer of filling operation.</returns>
        IObservable<string> FillRootFolderAsync(Folder parent, Dispatcher dispatcher);

        /// <summary>
        /// Check for directory existence on client machine.
        /// </summary>
        /// <param name="directoryPath">The full path.</param>
        /// <returns>Result of checking.</returns>
        bool IsDirectoryExists(string directoryPath);
    }
}