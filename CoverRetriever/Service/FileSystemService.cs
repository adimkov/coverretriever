// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemService.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Service for access to file system
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Service
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using CoverRetriever.AudioInfo;
    using CoverRetriever.Infrastructure;
    using CoverRetriever.Model;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Service for access to file system.
    /// </summary>
    [Export(typeof(IFileSystemService))]
    public class FileSystemService : IFileSystemService
    {
        /// <summary>
        /// Holds reference to service locator.
        /// </summary>
        private readonly IServiceLocator _serviceLocator;

        /// <summary>
        /// Fill root folder observer.
        /// </summary>
        private Subject<FileSystemItem> _fillRootSubject;

            /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemService"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        [ImportingConstructor]
        public FileSystemService(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        /// <summary>
        /// Gets children elements of specified folder.
        /// </summary>
        /// <param name="parent">The root folder.</param>
        /// <returns>Observer of filling operation.</returns>
        public IObservable<FileSystemItem> GetChildrenForRootFolder(Folder parent)
        {
            _fillRootSubject = new Subject<FileSystemItem>();
            
            return _fillRootSubject.Defer(() => GetFileSystemItems(parent, true));
        }

        /// <summary>
        /// Check for directory existence on client machine.
        /// </summary>
        /// <param name="directoryPath">
        /// Full path.
        /// </param>
        /// <returns>
        /// <c>true</c> if directory exists; otherwise - false
        /// </returns>
        public bool IsDirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        /// <summary>
        /// Gets the file system items.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="isRoot">Flag indicating that current folder is root.</param>
        private void GetFileSystemItems(Folder parent, bool isRoot)
        {
            var parentFullPath = parent.GetFileSystemItemFullPath();
            Debug.WriteLine(parentFullPath);

            try
            {
                var directories = Directory.GetDirectories(parentFullPath)
                    .Select(name => new Folder(Path.GetFileName(name), parent)).ToList();

                AddItemsToFolder(parent, isRoot, directories);

                var files =
                    Directory.GetFiles(parentFullPath)
                        .Where(file => AudioFormat.AudioFileExtensions.Any(ext => file.ToLower().EndsWith(ext)))
                        .Select(name =>
                                new AudioFile(
                                    Path.GetFileName(name),
                                    parent,
                                    ActivateIMetaProvider(name),
                                    ActivateDirectoryCoverOrganizer(name)));

                AddItemsToFolder(parent, isRoot, files);

                foreach (var folder in directories)
                {
                    GetFileSystemItems(folder, false);
                }

                if (isRoot)
                {
                    _fillRootSubject.OnCompleted();
                }
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("File access exception: '{0}'", parentFullPath);
            }
        }

        /// <summary>
        /// Adds the items to folder.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="isRoot">If set to <c>true</c> then parent is root.</param>
        /// <param name="children">The children.</param>
        private void AddItemsToFolder(Folder parent, bool isRoot, IEnumerable<FileSystemItem> children)
        {
            if (isRoot)
            {
                children.ForEach(x => _fillRootSubject.OnNext(x));
            }
            else
            {
                parent.Children.AddRange(children);
            }
        }

        /// <summary>
        /// Activates the I meta provider.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Lazy initialized Tag provider.</returns>
        private Lazy<IMetaProvider> ActivateIMetaProvider(string filePath)
        {
            var fileExtension = Path.GetExtension(filePath).TrimStart('.').ToLower();
            var metaProvider = new Lazy<IMetaProvider>(
                () =>
                {
                    var activator = _serviceLocator.GetInstance<IMetaProvider>(fileExtension) as IActivator;
                    if (activator == null)
                    {
                        throw new MetaProviderException("Unable to activate meta provider, activator was not found");
                    }

                    activator.Activate(filePath);
                    return (IMetaProvider)activator;
                });
            return metaProvider;
        }

        /// <summary>
        /// Activate <see cref="DirectoryCoverOrganizer"/> by file path.
        /// </summary>
        /// <param name="filePath">Full audio file path.</param>
        /// <returns>Service to save manipulate cover in the directory.</returns>
        private DirectoryCoverOrganizer ActivateDirectoryCoverOrganizer(string filePath)
        {
            var coverOrganizer = _serviceLocator.GetInstance<DirectoryCoverOrganizer>();
            coverOrganizer.Activate(Path.GetDirectoryName(filePath));
            return coverOrganizer;
        }
    }
}