using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Threading;

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
		//todo:Remove stub
		private readonly Folder _rootFolder = new RootFolder(@"g:\Ìóçûêà\ÄÄÒ");
		private AudioFile _fileDetails;
		private ObservableCollection<RemoteCover> _suggestedCovers = new ObservableCollection<RemoteCover>();

		[ImportingConstructor]
		public CoverRetrieverViewModel(IFileSystemService fileSystemService, ICoverRetrieverService coverRetrieverService)
		{
			_fileSystemService = fileSystemService;
			_coverRetrieverService = coverRetrieverService;
//			_rootFolder = rootFolder;
			LoadedCommand = new DelegateCommand(LoadedCommandExecute);
			FileSystemSelectedItemChangedCommand = new DelegateCommand<FileSystemItem>(FileSystemSelectedItemChangedCommandExecute);
			PreviewCoverCommand = new DelegateCommand<RemoteCover>(PreviewCoverCommandExecute);
			SaveCoverCommand = new DelegateCommand<RemoteCover>(SaveCoverCommandExecute);

			PreviewCoverRequest = new InteractionRequest<Notification>();
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
		/// Preview cover dialog request
		/// </summary>
		public InteractionRequest<Notification> PreviewCoverRequest { get; private set; }

		/// <summary>
		/// Get file system items
		/// </summary>
		public ObservableCollection<FileSystemItem> FileSystem
		{
			get { return _rootFolder.Children; }
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

		private void LoadedCommandExecute()
		{
			_fileSystemService.FillRootFolderAsync(_rootFolder, Dispatcher.CurrentDispatcher, null);
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
			PreviewCoverRequest.Raise(new Notification
			{
				Title = "Cover of album {0} - {1}".FormatUIString(FileDetails.Artist, FileDetails.Album),
				Content = remoteCover
			});
		}

		private void SaveCoverCommandExecute(RemoteCover remoteCover)
		{
			StartOperation(CoverRetrieverResources.MessageSaveCover);

			_coverRetrieverService.DownloadCover(remoteCover.CoverUri)
				.Finally(EndOperation)
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
				.Subscribe(x => SuggestedCovers.AddRange(x));
		}
	}
}