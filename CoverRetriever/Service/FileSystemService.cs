using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

using CoverRetriever.Model;

using Microsoft.Practices.ServiceLocation;

namespace CoverRetriever.Service
{
	[Export(typeof(IFileSystemService))]
	public class FileSystemService : IFileSystemService
	{
		private const string AudioFilesPatern = "*.mp3";
		
		private MetaProviderFactory _metaProviderFactory;
		private IServiceLocator _serviceLocator;
		private int _countRequestOfAddItems;
		private Action _onComplete;

		[ImportingConstructor]
		public FileSystemService(MetaProviderFactory metaProviderFactory, IServiceLocator serviceLocator)
		{
			_metaProviderFactory = metaProviderFactory;
			_serviceLocator = serviceLocator;
		}

		public void FillRootFolderAsync(Folder parent, Dispatcher dispatcher, Action onComplete)
		{
			ThreadPool.QueueUserWorkItem(
				state => 
				{
					_onComplete = onComplete;
					GetFileSystemItems(parent, dispatcher);
				});
		}

		private void GetFileSystemItems(Folder parent, Dispatcher dispatcher)
		{
			_countRequestOfAddItems += 2;

			var directories = Directory.GetDirectories(parent.GetFileSystemItemFullPath())
				.Select(name => new Folder(Path.GetFileName(name), parent)).ToList();

			if (dispatcher != null)
			{
				dispatcher.BeginInvoke(DispatcherPriority.Send, new AddItemsToFolderDelegate((AddItemsToFolder)), parent, directories);	
			}
			else
			{
				AddItemsToFolder(parent, directories);
			}

			var files = Directory.GetFiles(parent.GetFileSystemItemFullPath(), AudioFilesPatern)
				.Select(name =>
					new AudioFile(
						Path.GetFileName(name),
						parent,
						_metaProviderFactory.GetMetaProviderForFile(name),
						_serviceLocator.GetAllInstances(typeof(ICoverOrganizer)).Cast<ICoverOrganizer>().ToList()));

			if (dispatcher != null)
			{
				dispatcher.BeginInvoke(DispatcherPriority.Send, new AddItemsToFolderDelegate((AddItemsToFolder)), parent, files);
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

		private void AddItemsToFolder(Folder parent, IEnumerable<FileSystemItem> children)
		{
			_countRequestOfAddItems -= 1;
			parent.Children.AddRange(children);
			if (_countRequestOfAddItems == 0 && _onComplete != null)
			{
				_onComplete();
			}
		}

		private delegate void AddItemsToFolderDelegate(Folder parent, IEnumerable<FileSystemItem> children);
	}
}