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
        /// Count request of add items.
        /// </summary>
        private int _countRequestOfAddItems;

        /// <summary>
        /// Hold reference on complete action.
        /// </summary>
        private Action _onComplete;

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
        /// Recursive loads audio files and catalog list into parent <see cref="Folder"/>.
        /// <remarks>
        /// IF dispatcher is null, fill folder in current thread.
        /// </remarks>
        /// </summary>
        /// <param name="parent">Parent directory of file system items.</param>
        /// <param name="dispatcher">Syncronization context.</param>
        /// <param name="onComplete">Complete operation notify.</param>
        public void FillRootFolderAsync(Folder parent, Dispatcher dispatcher, Action onComplete)
        {
            ThreadPool.QueueUserWorkItem(
                state => 
                {
                    _onComplete = onComplete;
                    GetFileSystemItems(parent, dispatcher);
                });
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
        private void GetFileSystemItems(Folder parent, Dispatcher dispatcher)
        {
            _countRequestOfAddItems += 2;

            var directories = Directory.GetDirectories(parent.GetFileSystemItemFullPath())
                .Select(name => new Folder(Path.GetFileName(name), parent)).ToList();

            if (dispatcher != null)
            {
                dispatcher.BeginInvoke(DispatcherPriority.Send, new AddItemsToFolderDelegate(AddItemsToFolder), parent, directories);
            }
            else
            {
                AddItemsToFolder(parent, directories);
            }

            var files =
                Directory.GetFiles(parent.GetFileSystemItemFullPath()).Where(
                    file => AudioFormat.AudioFileExtensions.Any(ext => file.ToLower().EndsWith(ext))).Select(name =>
                    new AudioFile(
                        Path.GetFileName(name),
                        parent,
                        ActivateIMetaProvider(name),
                        ActivateDirectoryCoverOrganizer(name)));

            if (dispatcher != null)
            {
                dispatcher.BeginInvoke(DispatcherPriority.Send, new AddItemsToFolderDelegate(AddItemsToFolder), parent, files);
            }
            else
            {
                AddItemsToFolder(parent, files);
            }

            foreach (var folder in directories)
            {
                GetFileSystemItems(folder, dispatcher);
            }
        }

        /// <summary>
        /// Adds the items to folder.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="children">The children.</param>
        private void AddItemsToFolder(Folder parent, IEnumerable<FileSystemItem> children)
        {
            _countRequestOfAddItems -= 1;
            parent.Children.AddRange(children);
            if (_countRequestOfAddItems == 0 && _onComplete != null)
            {
                _onComplete();
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