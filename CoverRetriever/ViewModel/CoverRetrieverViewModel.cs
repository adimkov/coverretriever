using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using CoverRetriever.Interaction;
using CoverRetriever.Model;
using CoverRetriever.Resources;
using CoverRetriever.Service;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

using Notification = Microsoft.Practices.Prism.Interactivity.InteractionRequest.Notification;

namespace CoverRetriever.ViewModel
{
	[Export]
	public class CoverRetrieverViewModel : ViewModelBase
	{
		private const int SuggestedCountOfCovers = 5;

		private readonly IFileSystemService _fileSystemService;
		private readonly ICoverRetrieverService _coverRetrieverService;
		private readonly OpenFolderViewModel _openFolderViewModel;
		private Folder _rootFolder;
		private AudioFile _fileDetails;
		private ObservableCollection<RemoteCover> _suggestedCovers = new ObservableCollection<RemoteCover>();
		private RemoteCover _selectedSuggestedCover;
		private FileSystemItem _selectedFileSystemItem;
		

		[ImportingConstructor]
		public CoverRetrieverViewModel(IFileSystemService fileSystemService, ICoverRetrieverService coverRetrieverService, OpenFolderViewModel openFolderViewModel)
		{
			_fileSystemService = fileSystemService;
			_coverRetrieverService = coverRetrieverService;
			_openFolderViewModel = openFolderViewModel;
			openFolderViewModel.PushRootFolder.Subscribe(OnNext_PushRootFolder);
			LoadedCommand = new DelegateCommand(LoadedCommandExecute);
			FileSystemSelectedItemChangedCommand = new DelegateCommand<FileSystemItem>(FileSystemSelectedItemChangedCommandExecute);
			PreviewCoverCommand = new DelegateCommand<RemoteCover>(PreviewCoverCommandExecute);
			SaveCoverCommand = new DelegateCommand<RemoteCover>(SaveCoverCommandExecute);
			SelectFolderCommand = new DelegateCommand(SelectFolderCommandExecute);
			FinishCommand = new DelegateCommand(FinishCommandExecute);

			PreviewCoverRequest = new InteractionRequest<Notification>();
			SelectRootFolderRequest = new InteractionRequest<Notification>();
		}

		

		/// <summary>
		/// Loaded window Command
		/// </summary>
		public DelegateCommand LoadedCommand { get; private set; }
		
		/// <summary>
		/// Selection changed in file System tree view command
		/// </summary>
		public DelegateCommand<FileSystemItem> FileSystemSelectedItemChangedCommand { get; private set; }
		
		/// <summary>
		/// Preview selected cover
		/// </summary>
		public DelegateCommand<RemoteCover> PreviewCoverCommand { get; private set; }
		
		/// <summary>
		/// Save selected cover in to selected directory
		/// </summary>
		public DelegateCommand<RemoteCover> SaveCoverCommand { get; private set; }

		/// <summary>
		/// Change root directory command
		/// </summary>
		public DelegateCommand SelectFolderCommand { get; private set; }

		/// <summary>
		/// Finish work of application
		/// </summary>
		public DelegateCommand FinishCommand { get; private set; }

		/// <summary>
		/// Preview cover dialog request
		/// </summary>
		public InteractionRequest<Notification> PreviewCoverRequest { get; private set; }
		
		/// <summary>
		/// 
		/// </summary>
		public InteractionRequest<Notification> SelectRootFolderRequest { get; private set; }

		/// <summary>
		/// Get file system items
		/// </summary>
		public IEnumerable<FileSystemItem> FileSystem
		{
			get
			{
				return Enumerable.Repeat(_rootFolder, 1);
			}
		}

		/// <summary>
		/// Set or get selected File system item
		/// </summary>
		public FileSystemItem SelectedFileSystemItem
		{
			get
			{
				return _selectedFileSystemItem;
			}
			set
			{
				_selectedFileSystemItem = value;
				RaisePropertyChanged("SelectedFileSystemItem");
			}
		}

		/// <summary>
		/// Get audio file details
		/// </summary>
		public AudioFile FileDetails
		{
			get
			{
				return _fileDetails;
			}
			private set
			{
				_fileDetails = value;
				RaisePropertyChanged("FileDetails");
			}
		}

		/// <summary>
		/// Get collection of found covers
		/// </summary>
		public ObservableCollection<RemoteCover> SuggestedCovers
		{
			get
			{
				return _suggestedCovers;
			}
			private set
			{
				_suggestedCovers = value;
				RaisePropertyChanged("SuggestedCovers");
			}
		}

		/// <summary>
		/// Get or set suggested cover
		/// </summary>
		public RemoteCover SelectedSuggestedCover
		{
			get
			{
				return _selectedSuggestedCover;
			}
			set 
			{
				_selectedSuggestedCover = value;
				RaisePropertyChanged("SelectedSuggestedCover");
			}
		}

		private void LoadedCommandExecute()
		{
			if (_rootFolder == null)
			{
				_openFolderViewModel.IsCloseEnable = false;
				SelectFolderCommand.Execute();
			}
		}

		private void OnNext_PushRootFolder(RootFolderResult rootFolderResult)
		{
			_rootFolder = new RootFolder(rootFolderResult.RootFolder);
			RaisePropertyChanged("FileSystem");
			_fileSystemService.FillRootFolderAsync(_rootFolder, Dispatcher.CurrentDispatcher, SelectFirstAutioFile);
			SelectRootFolderRequest.Raise(new CloseNotification());
		}

		private void SelectFirstAutioFile()
		{
			Dispatcher.CurrentDispatcher.Invoke(new Action(() => SelectedFileSystemItem = FindFirstAudioFile(_rootFolder)));
		}

		private void FileSystemSelectedItemChangedCommandExecute(FileSystemItem file)
		{
			var folder = file as Folder;
			if (folder != null)
			{
				FileDetails = (AudioFile)folder.Children.FirstOrDefault(x => x is AudioFile);
			}
			else
			{
				FileDetails = file as AudioFile;
			}

			if (FileDetails != null)
			{
				FindRemoteCovers(FileDetails);
			}
		}
		
		private void PreviewCoverCommandExecute(RemoteCover remoteCover)
		{
			var viewModel = new CoverPreviewViewModel(remoteCover);
			
			viewModel.SaveCover.Subscribe(
				x =>
				{
					viewModel.SetBusy(true, CoverRetrieverResources.MessageSaveCover);
					SaveRemoteCover(x, () => viewModel.SetBusy(false, CoverRetrieverResources.MessageSaveCover));
				});

			PreviewCoverRequest.Raise(new Notification
			{
				Title = "Cover of album {0} - {1}".FormatUIString(FileDetails.Artist, FileDetails.Album),
				Content = viewModel 
			});
		}

		private void SaveCoverCommandExecute(RemoteCover remoteCover)
		{
			StartOperation(CoverRetrieverResources.MessageSaveCover);
			SaveRemoteCover(remoteCover, EndOperation);
		}

		private void SelectFolderCommandExecute()
		{
			SelectRootFolderRequest.Raise(new Notification
			{
				Title = CoverRetrieverResources.TitleStepOne,
				Content = _openFolderViewModel
			});
		}

		private void FinishCommandExecute()
		{
			WindowHandler.CloseAllWindow();
		}
		
		private void SaveRemoteCover(RemoteCover remoteCover, Action onCompllete)
		{
			_coverRetrieverService.DownloadCover(remoteCover.CoverUri)
				.Finally(
					() =>
					{
						var swapFileDetails = FileDetails;
						FileDetails = null;
						FileDetails = swapFileDetails;
						onCompllete();
					})
				.Subscribe(
					stream =>
					{
						FileDetails.CoverOrganizer.Single(x => x is DirectoryCoverOrganizer)
							.SaveCover(stream, Path.GetFileName(remoteCover.CoverUri.AbsolutePath));
					});
		}

		private void FindRemoteCovers(AudioFile fileDetails)
		{
			StartOperation(CoverRetrieverResources.MessageDownloadCover);
			_suggestedCovers.Clear();
			_coverRetrieverService.GetCoverFor(fileDetails.Artist, fileDetails.Album, SuggestedCountOfCovers)
				.Finally(EndOperation)
				.Subscribe(
				x =>
				{
					SuggestedCovers.AddRange(x);
					SelectedSuggestedCover = _suggestedCovers.Max();
				});
		}

		private FileSystemItem FindFirstAudioFile(Folder rootFolder)
		{
			foreach (var child in rootFolder.Children)
			{
				var audioFile = child as AudioFile;
				var folder = child as Folder;
				if (audioFile != null)
				{
					return audioFile;
				}
				
				if (folder != null)
				{
					var folderWithAudioFile = FindFirstAudioFile(folder);
					if (folderWithAudioFile != null)
					{
						return folderWithAudioFile;
					}
				}
			}
			return null;
		}
	}
}