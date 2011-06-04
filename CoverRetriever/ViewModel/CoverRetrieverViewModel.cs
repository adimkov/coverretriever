using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Concurrency;
using System.Linq;
using System.Windows.Threading;
using CoverRetriever.AudioInfo;
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
		private string _coverRetrieveErrorMessage;
		private Subject<ProcessResult> _savingCoverResult = new Subject<ProcessResult>();
		private CoverRecipient _recipient;

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
			SaveCoverCommand = new DelegateCommand<RemoteCover>(SaveCoverCommandExecute, rc => String.IsNullOrEmpty(CoverRetrieveErrorMessage));
			SelectFolderCommand = new DelegateCommand(SelectFolderCommandExecute);
			FinishCommand = new DelegateCommand(FinishCommandExecute);
			AboutCommand = new DelegateCommand(AboutCommandExecute);
			CloseErrorMessage = new DelegateCommand(CloseErrorMessageExecute);

			PreviewCoverRequest = new InteractionRequest<Notification>();
			SelectRootFolderRequest = new InteractionRequest<Notification>();
			AboutRequest = new InteractionRequest<Notification>();
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
		/// About dialog command
		/// </summary>
		public DelegateCommand AboutCommand { get; private set; }

		/// <summary>
		/// Close an error message
		/// </summary>
		public DelegateCommand CloseErrorMessage { get; private set; }

		/// <summary>
		/// Preview cover dialog request
		/// </summary>
		public InteractionRequest<Notification> PreviewCoverRequest { get; private set; }
		
		/// <summary>
		/// Select new root folder dialog
		/// </summary>
		public InteractionRequest<Notification> SelectRootFolderRequest { get; private set; }

		/// <summary>
		/// About dialog
		/// </summary>
		public InteractionRequest<Notification> AboutRequest { get; set; }

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
		public virtual AudioFile FileDetails
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

		/// <summary>
		/// Get error message of retrieve cover from web
		/// </summary>
		public string CoverRetrieveErrorMessage
		{
			get
			{
				return _coverRetrieveErrorMessage;
			}
			private set
			{
				_coverRetrieveErrorMessage = value;
				SaveCoverCommand.RaiseCanExecuteChanged();
				RaisePropertyChanged("CoverRetrieveErrorMessage");
			}
		}

		/// <summary>
		/// Get push collection of Saving cover result
		/// </summary>
		public IObservable<ProcessResult> SavingCoverResult
		{
			get { return _savingCoverResult; }
			
		}

		/// <summary>
		/// Get or set <see cref="CoverRecipient">Recipient</see> to save <see cref="Cover"/>
		/// </summary>
		public CoverRecipient Recipient
		{
			get
			{
				return _recipient;
			}
			set
			{
				_recipient = value;
				RaisePropertyChanged("Recipient");
			}
		}

		[Import(typeof(AboutViewModel))]
		private Lazy<AboutViewModel> AboutViewModel { get; set; }

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

					SaveRemoteCover(
						x, 
						() => viewModel.SetBusy(false, CoverRetrieverResources.MessageSaveCover));
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
			_savingCoverResult.OnNext(ProcessResult.Begin);
			SaveRemoteCover(
				remoteCover,
				() =>
				{
					_savingCoverResult.OnNext(ProcessResult.Done);
					EndOperation();
				});
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

		private void AboutCommandExecute()
		{
			AboutRequest.Raise(new Notification
			{
				Content = AboutViewModel.Value
			});
		}

		private void CloseErrorMessageExecute()
		{
			CoverRetrieveErrorMessage = String.Empty;
		}

		/// <summary>
		/// Set error message of cover retrieve
		/// </summary>
		/// <param name="ex"></param>
		private void SetErrorCoverRetrieve(Exception ex)
		{
			CoverRetrieveErrorMessage = ex.Message;
		}

		/// <summary>
		/// Clear error message of cover retieve
		/// </summary>
		private void ResetErrorCoverRetrieve()
		{
			CoverRetrieveErrorMessage = String.Empty;
		}

		/// <summary>
		/// Save image of caver at disc
		/// </summary>
		/// <param name="remoteCover"></param>
		/// <param name="onCompllete"></param>
		private void SaveRemoteCover(RemoteCover remoteCover, Action onCompllete)
		{
			var recipient = Recipient == CoverRecipient.Folder ? FileDetails.DirectoryCover : FileDetails.FrameCover;
			recipient
				.SaveCover(remoteCover)
				.Finally(
					() =>
						{
							var swapFileDetails = FileDetails;
							FileDetails = null;
							FileDetails = swapFileDetails;
							onCompllete();
						})
				.Subscribe(
					x => { },
					ex => { CoverRetrieveErrorMessage = ex.Message; });
		}

		/// <summary>
		/// Perform search covers in web
		/// </summary>
		/// <param name="fileDetails"></param>
		private void FindRemoteCovers(AudioFile fileDetails)
		{
			StartOperation(CoverRetrieverResources.MessageDownloadCover);
			ResetErrorCoverRetrieve();
			_suggestedCovers.Clear();
			var albumCondition = fileDetails.Album;

			_coverRetrieverService.GetCoverFor(fileDetails.Artist, albumCondition, SuggestedCountOfCovers)
				.SubscribeOn(Scheduler.TaskPool)
				.ObserveOnDispatcher()
				.Finally(EndOperation)
				.Subscribe(
				x =>
				{
					SuggestedCovers.AddRange(x);
					SelectedSuggestedCover = _suggestedCovers.Max();
				},
				SetErrorCoverRetrieve);
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