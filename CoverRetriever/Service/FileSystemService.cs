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
    using System.Threading;
    using System.Windows.Threading;

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
        /// Hold reference on complete action.
        /// </summary>
        private Action _onComplete;

        /// <summary>
        /// Fill root folder observer.
        /// </summary>
        private Subject<string> _fillRootSubject;

            /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemService"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        [ImportingConstructor]
        public FileSystemService(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            _fillRootSubject = new Subject<string>();
        }

        /// <summary>
        /// Delegate to add files in folder in dispatcher.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="children">
        /// The children.
        /// </param>
        private delegate void AddItemsToFolderDelegate(Folder parent, IEnumerable<FileSystemItem> children);

        /// <summary>
        /// Fills the root folder with subfolders and audio files async.
        /// </summary>
        /// <param name="parent">The root folder.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <returns>Observer of operation.</returns>
        public IObservable<string> FillRootFolderAsync(Folder parent, Dispatcher dispatcher)
        {
            _fillRootSubject = new Subject<string>();

            return
                _fillRootSubject.Defer(
                    () => { ThreadPool.QueueUserWorkItem(state => GetFileSystemItems(parent, dispatcher, true)); });
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
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="isRoot">Flag indicating that current folder is root.</param>
        private void GetFileSystemItems(Folder parent, Dispatcher dispatcher, bool isRoot)
        {
            var parentFullPath = parent.GetFileSystemItemFullPath();
            _fillRootSubject.OnNext(parentFullPath);
            Debug.WriteLine(parentFullPath);

            var directories = Directory.GetDirectories(parent.GetFileSystemItemFullPath())
                .Select(name => new Folder(Path.GetFileName(name), parent)).ToList();

            AddFileSystemSafe(parent, dispatcher, directories);

            var files =
                Directory.GetFiles(parentFullPath)
                .Where(file => AudioFormat.AudioFileExtensions.Any(ext => file.ToLower().EndsWith(ext)))
                .Select(name =>
                    new AudioFile(
                        Path.GetFileName(name),
                        parent,
                        ActivateIMetaProvider(name),
                        ActivateDirectoryCoverOrganizer(name)));

            AddFileSystemSafe(parent, dispatcher, files);

            foreach (var folder in directories)
            {
                GetFileSystemItems(folder, dispatcher, false);
            }

            if (isRoot)
            {
                _fillRootSubject.OnCompleted();
                if (_onComplete != null)
                {
                    _onComplete();
                }    
            }
        }

        /// <summary>
        /// Adds the file system in specified folder.
        /// <remarks>
        /// If dispatcher specified - will be used dispatcher to add items, otherwise will be add without dispatcher.
        /// </remarks>
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="items">The items.</param>
        private void AddFileSystemSafe(Folder parent, Dispatcher dispatcher, IEnumerable<FileSystemItem> items)
        {
            if (dispatcher != null)
            {
                dispatcher.BeginInvoke(
                    DispatcherPriority.Send, new AddItemsToFolderDelegate(AddItemsToFolder), parent, items);
            }
            else
            {
                AddItemsToFolder(parent, items);
            }
        }

        /// <summary>
        /// Adds the items to folder.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="children">The children.</param>
        private void AddItemsToFolder(Folder parent, IEnumerable<FileSystemItem> children)
        {
            parent.Children.AddRange(children);
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