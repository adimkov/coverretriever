// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoverRetrieverViewModel.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  ViewModel for main window of application
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Concurrency;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Threading;

    using CoverRetriever.Interaction;
    using CoverRetriever.Model;
    using CoverRetriever.Resources;
    using CoverRetriever.Service;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

    using Notification = Microsoft.Practices.Prism.Interactivity.InteractionRequest.Notification;

    /// <summary>
    /// ViewModel for main window of application.
    /// </summary>
    [Export]
    public class CoverRetrieverViewModel : ViewModelBase
    {
        /// <summary>
        /// Count of suggested covers.
        /// </summary>
        private const int SuggestedCountOfCovers = 6;

        /// <summary>
        /// Delay time to check updates in sec.
        /// </summary>
        private const int DelayToCheckUpdate = 10;

        /// <summary>
        /// Service to access to at file system.
        /// </summary>
        private readonly IFileSystemService _fileSystemService;

        /// <summary>
        /// Service to grab album art from web.
        /// </summary>
        private readonly ICoverRetrieverService _coverRetrieverService;

        /// <summary>
        /// Result of saving operation.
        /// </summary>
        private readonly Subject<ProcessResult> _savingCoverResult = new Subject<ProcessResult>();

        /// <summary>
        /// View model of open folder.
        /// </summary>
        private readonly OpenFolderViewModel _openFolderViewModel;

        /// <summary>
        /// Found album arts.
        /// </summary>
        private readonly ObservableCollection<RemoteCover> _suggestedCovers = new ObservableCollection<RemoteCover>();

        /// <summary>
        /// Selected folder to analyze audio files.
        /// </summary>
        private Folder _rootFolder;

        /// <summary>
        /// Selected cover.
        /// </summary>
        private RemoteCover _selectedSuggestedCover;

        /// <summary>
        /// Selected folder of audio file.
        /// </summary>
        private FileSystemItem _selectedFileSystemItem;

        /// <summary>
        /// Error message.
        /// </summary>
        private string _coverRetrieveErrorMessage;

        /// <summary>
        /// New version of application.
        /// </summary>
        private string _newVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverRetrieverViewModel"/> class.
        /// </summary>
        /// <param name="fileSystemService">The file system service.</param>
        /// <param name="coverRetrieverService">The cover retriever service.</param>
        /// <param name="openFolderViewModel">The open folder view model.</param>
        /// <param name="fileConductorViewModel">The file conductor view model.</param>
        [ImportingConstructor]
        public CoverRetrieverViewModel(
            IFileSystemService fileSystemService, 
            ICoverRetrieverService coverRetrieverService, 
            OpenFolderViewModel openFolderViewModel,
            FileConductorViewModel fileConductorViewModel)
        {
            _fileSystemService = fileSystemService;
            _coverRetrieverService = coverRetrieverService;
            _openFolderViewModel = openFolderViewModel;
            FileConductorViewModel = fileConductorViewModel;

            openFolderViewModel.PushRootFolder.Subscribe(this.OnNextPushRootFolder);
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
        /// Gets Loaded window Command.
        /// </summary>
        public DelegateCommand LoadedCommand { get; private set; }
        
        /// <summary>
        /// Gets selection changed in file System tree view command.
        /// </summary>
        public DelegateCommand<FileSystemItem> FileSystemSelectedItemChangedCommand { get; private set; }
        
        /// <summary>
        /// Gets preview selected cover.
        /// </summary>
        public DelegateCommand<RemoteCover> PreviewCoverCommand { get; private set; }
        
        /// <summary>
        /// Gets save selected cover in to selected directory.
        /// </summary>
        public DelegateCommand<RemoteCover> SaveCoverCommand { get; private set; }

        /// <summary>
        /// Gets change root directory command.
        /// </summary>
        public DelegateCommand SelectFolderCommand { get; private set; }

        /// <summary>
        /// Gets finish work of application.
        /// </summary>
        public DelegateCommand FinishCommand { get; private set; }
        
        /// <summary>
        /// Gets about dialog command.
        /// </summary>
        public DelegateCommand AboutCommand { get; private set; }

        /// <summary>
        /// Gets close an error message.
        /// </summary>
        public DelegateCommand CloseErrorMessage { get; private set; }

        /// <summary>
        /// Gets preview cover dialog request.
        /// </summary>
        public InteractionRequest<Notification> PreviewCoverRequest { get; private set; }
        
        /// <summary>
        /// Gets select new root folder dialog.
        /// </summary>
        public InteractionRequest<Notification> SelectRootFolderRequest { get; private set; }

        /// <summary>
        /// Gets or sets the about request.
        /// </summary>
        /// <value>
        /// The about request.
        /// </value>
        public InteractionRequest<Notification> AboutRequest { get; set; }

        /// <summary>
        /// Gets the file system.
        /// </summary>
        public IEnumerable<FileSystemItem> FileSystem
        {
            get
            {
                return Enumerable.Repeat(_rootFolder, 1);
            }
        }

        /// <summary>
        /// Gets or sets the selected file system item.
        /// </summary>
        /// <value>
        /// The selected file system item.
        /// </value>
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
        /// Gets the file conductor view model.
        /// </summary>
        public FileConductorViewModel FileConductorViewModel { get; private set; }

        /// <summary>
        /// Gets the suggested covers.
        /// </summary>
       public ObservableCollection<RemoteCover> SuggestedCovers
        {
            get
            {
                return _suggestedCovers;
            }
        }

       /// <summary>
       /// Gets or sets the selected suggested cover.
       /// </summary>
       /// <value>
       /// The selected cover.
       /// </value>
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
        /// Gets the cover retrieve error message.
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
        /// Gets the new version.
        /// </summary>
        /// <remarks>
        /// If new version does not available - field will be as Empty string.
        /// </remarks>
        public string NewVersion
        {
            get
            {
                return _newVersion;
            }

            private set
            {
                _newVersion = value;
                this.RaisePropertyChanged("NewVersion");
            }
        }

        /// <summary>
        /// Gets the saving cover result.
        /// </summary>
        public IObservable<ProcessResult> SavingCoverResult
        {
            get { return _savingCoverResult; }
        }

        /// <summary>
        /// Gets or sets the about view model.
        /// </summary>
        /// <value>
        /// The about view model.
        /// </value>
        [Import(typeof(AboutViewModel))]
        private Lazy<AboutViewModel> AboutViewModel { get; set; }

        /// <summary>
        /// Gets or sets the version control service.
        /// </summary>
        /// <value>
        /// The version control.
        /// </value>
        [Import(typeof(IVersionControlService))]
        private Lazy<IVersionControlService> VersionControl { get; set; }

        /// <summary>
        /// Loadeds the command execute.
        /// </summary>
        private void LoadedCommandExecute()
        {
            if (_rootFolder == null)
            {
                _openFolderViewModel.IsCloseEnabled = false;
                SelectFolderCommand.Execute();
                VersionControl.Value
                    .GetLatestVersion()
                    .Delay(TimeSpan.FromSeconds(DelayToCheckUpdate))
                    .Subscribe(GetLatestApplicatioVersion);
            }
        }

        /// <summary>
        /// Called when user choose folder.
        /// </summary>
        /// <param name="rootFolderResult">The root folder result.</param>
        private void OnNextPushRootFolder(RootFolderResult rootFolderResult)
        {
            _rootFolder = new RootFolder(rootFolderResult.RootFolder);
            RaisePropertyChanged("FileSystem");
            _fileSystemService.FillRootFolderAsync(_rootFolder, Dispatcher.CurrentDispatcher, this.SelectFirstAudioFile);
            SelectRootFolderRequest.Raise(new CloseNotification());
        }

        /// <summary>
        /// Selects the first audio file.
        /// </summary>
        private void SelectFirstAudioFile()
        {
            Dispatcher.CurrentDispatcher.Invoke(new Action(() => SelectedFileSystemItem = FindFirstAudioFile(_rootFolder)));
        }

        /// <summary>
        /// Called when selected folder or audio file changed.
        /// </summary>
        /// <param name="file">The file or folder.</param>
        private void FileSystemSelectedItemChangedCommandExecute(FileSystemItem file)
        {
            var folder = file as Folder;
            if (folder != null)
            {
                FileConductorViewModel.SelectedAudio = (AudioFile)folder.Children.FirstOrDefault(x => x is AudioFile);
            }
            else
            {
                FileConductorViewModel.SelectedAudio = file as AudioFile;
            }

            if (FileConductorViewModel.SelectedAudio != null)
            {
                FindRemoteCovers(FileConductorViewModel.SelectedAudio);
            }
        }

        /// <summary>
        /// Called when preview command executes.
        /// </summary>
        /// <param name="remoteCover">The remote cover.</param>
        private void PreviewCoverCommandExecute(RemoteCover remoteCover)
        {
            var viewModel = new CoverPreviewViewModel(remoteCover);
            
            viewModel.SaveCover.Subscribe(
                x =>
                {
                    viewModel.SetBusy(true, CoverRetrieverResources.MessageSaveCover);

                    SaveRemoteCover(x)
                        .Finally(() => viewModel.SetBusy(false, CoverRetrieverResources.MessageSaveCover))
                        .Subscribe();
                });

            PreviewCoverRequest.Raise(new Notification
            {
                Title = "Cover of album {0} - {1}".FormatUIString(
                FileConductorViewModel.SelectedAudio.Artist, 
                FileConductorViewModel.SelectedAudio.Album),
                Content = viewModel 
            });
        }

        /// <summary>
        /// Called when save command execute.
        /// </summary>
        /// <param name="remoteCover">The remote cover.</param>
        private void SaveCoverCommandExecute(RemoteCover remoteCover)
        {
            StartOperation(CoverRetrieverResources.MessageSaveCover);
            _savingCoverResult.OnNext(ProcessResult.Begin);
            SaveRemoteCover(remoteCover)
                .Finally(() =>
                {
                    _savingCoverResult.OnNext(ProcessResult.Done);
                    EndOperation();
                })
                .Subscribe();
        }

        /// <summary>
        /// Called when select the folder command executes.
        /// </summary>
        private void SelectFolderCommandExecute()
        {
            SelectRootFolderRequest.Raise(new Notification
            {
                Title = CoverRetrieverResources.TitleStepOne,
                Content = _openFolderViewModel
            });
        }

        /// <summary>
        /// Called when finished command executes.
        /// </summary>
        private void FinishCommandExecute()
        {
            WindowHandler.CloseAllWindow();
        }

        /// <summary>
        /// Called when about command executes.
        /// </summary>
        private void AboutCommandExecute()
        {
            AboutRequest.Raise(new Notification
            {
                Content = AboutViewModel.Value
            });
        }

        /// <summary>
        /// Called when error message close executes.
        /// </summary>
        private void CloseErrorMessageExecute()
        {
            CoverRetrieveErrorMessage = String.Empty;
        }

        /// <summary>
        /// Sets the error cover retrieve.
        /// </summary>
        /// <param name="ex">The exception.</param>
        private void SetErrorCoverRetrieve(Exception ex)
        {
            CoverRetrieveErrorMessage = ex.Message;
        }

        /// <summary>
        /// Clear error message of cover retiever.
        /// </summary>
        private void ResetErrorCoverRetrieve()
        {
            CoverRetrieveErrorMessage = String.Empty;
        }

        /// <summary>
        /// Save image of caver onto disc.
        /// </summary>
        /// <param name="remoteCover">The remote cover.</param>
        /// <returns>Operation observable.</returns>
        private IObservable<Unit> SaveRemoteCover(RemoteCover remoteCover)
        {
            return FileConductorViewModel.SaveCover(remoteCover)
                .Do(x => { }, ex => { CoverRetrieveErrorMessage = ex.Message; })
                .OnErrorResumeNext(Observable.Empty<Unit>());
        }

        /// <summary>
        /// Perform search covers in web for audio.
        /// </summary>
        /// <param name="fileDetails">The audio file.</param>
        private void FindRemoteCovers(AudioFile fileDetails)
        {
            StartOperation(CoverRetrieverResources.MessageDownloadCover);
            ResetErrorCoverRetrieve();
            _suggestedCovers.Clear();
            var albumCondition = fileDetails.Album;

            _coverRetrieverService.GetCoverFor(fileDetails.Artist, albumCondition, SuggestedCountOfCovers)
                .SubscribeOn(Scheduler.ThreadPool)
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

        /// <summary>
        /// Finds the first audio file in folder.
        /// </summary>
        /// <param name="rootFolder">The root folder.</param>
        /// <returns>First audio file.</returns>
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

        /// <summary>
        /// Called when the latest version of application available.
        /// </summary>
        /// <param name="revisionVersion">The revision version.</param>
        private void GetLatestApplicatioVersion(RevisionVersion revisionVersion)
        {
            if (Assembly.GetExecutingAssembly().GetName().Version < revisionVersion.Version)
            {
                NewVersion = CoverRetrieverResources.TextNewVersion.FormatUIString(revisionVersion.Version);
            }
        }
    }
}