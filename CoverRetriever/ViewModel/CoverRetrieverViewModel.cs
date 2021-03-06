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
    using System.Collections.Specialized;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Reflection;
    using System.Windows.Input;

    using CoverRetriever.AudioInfo;
    using CoverRetriever.Common.Interaction;
    using CoverRetriever.Common.ViewModel;
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
        private const int DelayToCheckUpdate = 4;

        /// <summary>
        /// Service to access to at file system.
        /// </summary>
        private readonly IFileSystemService fileSystemService;

        /// <summary>
        /// Service to grab album art from web.
        /// </summary>
        private readonly ICoverRetrieverService coverRetrieverService;

        /// <summary>
        /// Backing field for Loaded command.
        /// </summary>
        private readonly DelegateCommand loadedCommand;

        /// <summary>
        /// Backing field for FileSystemSelectedItemChanged command.
        /// </summary>
        private readonly DelegateCommand<FileSystemItem> fileSystemSelectedItemChangedCommand;

        /// <summary>
        /// Backing field for PreviewCommand command.
        /// </summary>
        private readonly DelegateCommand<RemoteCover> previewCoverCommand;

        /// <summary>
        /// Backing field for SaveCover command.
        /// </summary>
        private readonly DelegateCommand<RemoteCover> saveCoverCommand;

        /// <summary>
        /// Backing field for SelectFolder command.
        /// </summary>
        private readonly DelegateCommand selectFolderCommand;

        /// <summary>
        /// Backing field for Finish command.
        /// </summary>
        private readonly DelegateCommand finishCommand;

        /// <summary>
        /// Backing field for About command.
        /// </summary>
        private readonly DelegateCommand aboutCommand;
        
        /// <summary>
        /// Backing field for CloseError command.
        /// </summary>
        private readonly DelegateCommand closeErrorMessage;

        /// <summary>
        /// The request to enlarge selected cover.
        /// </summary>
        private readonly InteractionRequest<Notification> previewCoverRequest;

        /// <summary>
        /// The request to open library window.
        /// </summary>
        private readonly InteractionRequest<Notification> selectRootFolderRequest;

        /// <summary>
        /// The request to show about window.
        /// </summary>
        private readonly InteractionRequest<Notification> aboutRequest;

        /// <summary>
        /// Result of saving operation.
        /// </summary>
        private readonly Subject<ProcessResult> savingCoverResult = new Subject<ProcessResult>();

        /// <summary>
        /// View model of open folder.
        /// </summary>
        private readonly OpenFolderViewModel openFolderViewModel;

        /// <summary>
        /// Found album arts.
        /// </summary>
        private readonly ObservableCollection<RemoteCover> suggestedCovers = new ObservableCollection<RemoteCover>();

        /// <summary>
        /// Selected folder to analyze audio files.
        /// </summary>
        private Folder rootFolder;

        /// <summary>
        /// Selected cover.
        /// </summary>
        private RemoteCover selectedSuggestedCover;

        /// <summary>
        /// Selected folder of audio file.
        /// </summary>
        private FileSystemItem selectedFileSystemItem;

        /// <summary>
        /// Error message.
        /// </summary>
        private string coverRetrieverErrorMessage;

        /// <summary>
        /// New version of application.
        /// </summary>
        private string newVersion;

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
            this.fileSystemService = fileSystemService;
            this.coverRetrieverService = coverRetrieverService;
            this.openFolderViewModel = openFolderViewModel;
            FileConductorViewModel = fileConductorViewModel;
            FileConductorViewModel.ParentViewModel = this;
            this.fileSystemService = fileSystemService;

            openFolderViewModel.PushRootFolder.Subscribe(OnNextPushRootFolder);
            loadedCommand = new DelegateCommand(LoadedCommandExecute);
            fileSystemSelectedItemChangedCommand = new DelegateCommand<FileSystemItem>(FileSystemSelectedItemChangedCommandExecute);
            previewCoverCommand = new DelegateCommand<RemoteCover>(PreviewCoverCommandExecute);
            saveCoverCommand = new DelegateCommand<RemoteCover>(SaveCoverCommandExecute, CanExecuteSaveCoverCommand);
            selectFolderCommand = new DelegateCommand(SelectFolderCommandExecute);
            finishCommand = new DelegateCommand(FinishCommandExecute);
            aboutCommand = new DelegateCommand(AboutCommandExecute);
            closeErrorMessage = new DelegateCommand(CloseErrorMessageExecute);
            
            previewCoverRequest = new InteractionRequest<Notification>();
            selectRootFolderRequest = new InteractionRequest<Notification>();
            aboutRequest = new InteractionRequest<Notification>();

            suggestedCovers.CollectionChanged += SuggestedCoversOnCollectionChanged;

            ObservableScheduler = DispatcherScheduler.Instance;
            SubscribeScheduler = Scheduler.ThreadPool;
        }

        /// <summary>
        /// Gets Loaded window Command.
        /// </summary>
        public ICommand LoadedCommand
        {
            get
            {
                return loadedCommand;
            }
        }

        /// <summary>
        /// Gets selection changed in file System tree view command.
        /// </summary>
        public ICommand FileSystemSelectedItemChangedCommand
        {
            get
            {
                return fileSystemSelectedItemChangedCommand;
            }
        }
        
        /// <summary>
        /// Gets preview selected cover.
        /// </summary>
        public ICommand PreviewCoverCommand       
        {
            get
            {
                return previewCoverCommand;
            }
        }
        
        /// <summary>
        /// Gets save selected cover in to selected directory.
        /// </summary>
        public ICommand SaveCoverCommand
        {
            get
            {
                return saveCoverCommand;
            }
        }

        /// <summary>
        /// Gets change root directory command.
        /// </summary>
        public ICommand SelectFolderCommand
        {
            get
            {
                return selectFolderCommand;
            }
        }

        /// <summary>
        /// Gets finish work of application.
        /// </summary>
        public ICommand FinishCommand
        {
            get
            {
                return finishCommand;
            }
        }
        
        /// <summary>
        /// Gets about dialog command.
        /// </summary>
        public ICommand AboutCommand
        {
            get
            {
                return aboutCommand;
            }
        }

        /// <summary>
        /// Gets close an error message.
        /// </summary>
        public ICommand CloseErrorMessage
        {
            get
            {
                return closeErrorMessage;
            }
        }

        /// <summary>
        /// Gets preview cover dialog request.
        /// </summary>
        public IInteractionRequest PreviewCoverRequest
        {
            get
            {
                return previewCoverRequest;
            }
        }
        
        /// <summary>
        /// Gets select new root folder dialog.
        /// </summary>
        public IInteractionRequest SelectRootFolderRequest
        {
            get
            {
                return selectRootFolderRequest;
            }
        }

        /// <summary>
        /// Gets the about request.
        /// </summary>
        /// <value>
        /// The about request.
        /// </value>
        public IInteractionRequest AboutRequest
        {
            get
            {
                return aboutRequest;
            }
        }

        /// <summary>
        /// Gets or sets the scheduler to execute observable.
        /// </summary>
        /// <value>
        /// The scheduler.
        /// </value>
        public IScheduler ObservableScheduler { get; set; }

        /// <summary>
        /// Gets or sets the subscribe scheduler.
        /// </summary>
        /// <value>
        /// The subscribe scheduler.
        /// </value>
        public IScheduler SubscribeScheduler { get; set; }

        /// <summary>
        /// Gets the file system.
        /// </summary>
        public IEnumerable<FileSystemItem> FileSystem
        {
            get
            {
                return Enumerable.Repeat(rootFolder, 1);
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
                return selectedFileSystemItem;
            }

            set
            {
                selectedFileSystemItem = value;
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
                return suggestedCovers;
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
                return selectedSuggestedCover;
            }

            set 
            {
                selectedSuggestedCover = value;
                RaisePropertyChanged("SelectedSuggestedCover");
            }
        }

        /// <summary>
        /// Gets the cover retrieve error message.
        /// </summary>
        public string CoverRetrieverErrorMessage
        {
            get
            {
                return coverRetrieverErrorMessage;
            }

            private set
            {
                coverRetrieverErrorMessage = value;
                saveCoverCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged("CoverRetrieverErrorMessage");
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
                return newVersion;
            }

            private set
            {
                newVersion = value;
                RaisePropertyChanged("NewVersion");
            }
        }

        /// <summary>
        /// Gets the saving cover result.
        /// </summary>
        public IObservable<ProcessResult> SavingCoverResult
        {
            get { return savingCoverResult; }
        }

        /// <summary>
        /// Gets or sets the about view model.
        /// </summary>
        /// <value>
        /// The about view model.
        /// </value>
        [Import(typeof(AboutViewModel))]
        public Lazy<AboutViewModel> AboutViewModel { get; set; }

        /// <summary>
        /// Gets or sets the version control service.
        /// </summary>
        /// <value>
        /// The version control.
        /// </value>
        [Import(typeof(IVersionControlService))]
        public Lazy<IVersionControlService> VersionControl { get; set; }

        /// <summary>
        /// Perform search covers in web for audio.
        /// </summary>
        /// <param name="fileDetails">The audio file.</param>
        internal void FindRemoteCovers(IMetaProvider fileDetails)
        {
            Trace.WriteLine("Get covers for: [{0}]".FormatString(fileDetails));
            StartOperation(CoverRetrieverResources.MessageDownloadCover);
            ResetError();
            var albumCondition = fileDetails.Album;

            coverRetrieverService.GetCoverFor(fileDetails.Artist, albumCondition, SuggestedCountOfCovers)
                .SubscribeOn(SubscribeScheduler)
                .ObserveOn(ObservableScheduler)
                .Finally(EndOperation)
                .Subscribe(
                x =>
                {
                    SuggestedCovers.Clear();
                    SuggestedCovers.AddRange(x);
                    SelectedSuggestedCover = suggestedCovers.Max();
                },
                SetError);
        }

        /// <summary>
        /// Sets the error cover retrieve.
        /// </summary>
        /// <param name="ex">The exception.</param>
        internal void SetError(Exception ex)
        {
            CoverRetrieverErrorMessage = ex.Message;
        }

        /// <summary>
        /// Loads the command execute.
        /// </summary>
        private void LoadedCommandExecute()
        {
            if (rootFolder == null)
            {
                openFolderViewModel.IsCloseEnabled = false;
                selectFolderCommand.Execute();
                VersionControl.Value
                    .GetLatestVersion()
                    .Delay(TimeSpan.FromSeconds(DelayToCheckUpdate))
                    .Catch<RevisionVersion, WebException>(x => Observable.Empty<RevisionVersion>())
                    .Subscribe(GetLatestApplicatioVersion);
            }
        }

        /// <summary>
        /// Called when user choose folder.
        /// </summary>
        /// <param name="rootFolderResult">The root folder result.</param>
        private void OnNextPushRootFolder(RootFolderResult rootFolderResult)
        {
            rootFolder = new RootFolder(rootFolderResult.RootFolder);
            RaisePropertyChanged("FileSystem");
            StartOperation(CoverRetrieverResources.MessageLibraryLoad);
            fileSystemService.GetChildrenForRootFolder(rootFolder)
                .SubscribeOn(SubscribeScheduler)
                .ObserveOn(ObservableScheduler)
                .Subscribe(
                x => rootFolder.Children.Add(x),
                SelectFirstAudioFile);
            selectRootFolderRequest.Raise(new CloseNotification());
        }

        /// <summary>
        /// Selects the first audio file.
        /// </summary>
        private void SelectFirstAudioFile()
        {
            EndOperation();
            SelectedFileSystemItem = FindFirstAudioFile(rootFolder);
        }

        /// <summary>
        /// Called when selected folder or audio file changed.
        /// </summary>
        /// <param name="file">The file or folder.</param>
        private void FileSystemSelectedItemChangedCommandExecute(FileSystemItem file)
        {
            SelectedFileSystemItem = file;
            SetAudioFile(file);
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

            previewCoverRequest.Raise(new Notification
            {
                Title = CoverRetrieverResources.CoverPreviewTitle.FormatUIString(
                FileConductorViewModel.Artist, 
                FileConductorViewModel.Album),
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
            savingCoverResult.OnNext(ProcessResult.Begin);
            SaveRemoteCover(remoteCover)
                .Finally(() =>
                {
                    savingCoverResult.OnNext(ProcessResult.Done);
                    EndOperation();
                })
                .Subscribe();
        }

        /// <summary>
        /// Called when select the folder command executes.
        /// </summary>
        private void SelectFolderCommandExecute()
        {
            selectRootFolderRequest.Raise(new Notification
            {
                Title = CoverRetrieverResources.TitleStepOne,
                Content = openFolderViewModel
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
            aboutRequest.Raise(new Notification
            {
                Content = AboutViewModel.Value
            });
        }

        /// <summary>
        /// Called when error message close executes.
        /// </summary>
        private void CloseErrorMessageExecute()
        {
            CoverRetrieverErrorMessage = String.Empty;
        }

        /// <summary>
        /// Clear error message of cover retriever.
        /// </summary>
        private void ResetError()
        {
            CoverRetrieverErrorMessage = String.Empty;
        }

        /// <summary>
        /// Sets the audio file to the file conductor view.
        /// </summary>
        /// <param name="file">The file.</param>
        private void SetAudioFile(FileSystemItem file)
        {
            var folder = file as Folder;
            var audio = file as AudioFile;
            if (folder != null)
            {
                audio = (AudioFile)folder.Children.FirstOrDefault(x => x is AudioFile);
            }
            
            FileConductorViewModel.SelectedAudio = audio;
            
            if (audio != null)
            {
                if (FileConductorViewModel.IsNeededToRetrieveTags)
                {
                    FileConductorViewModel.HighlightToGetTags.Raise(null);
                }

                FindRemoteCovers(audio.MetaProvider);
            }
            else
            {
                SuggestedCovers.Clear();
            }
        }

        /// <summary>
        /// Save image of caver onto disc.
        /// </summary>
        /// <param name="remoteCover">The remote cover.</param>
        /// <returns>Operation observable.</returns>
        private IObservable<Unit> SaveRemoteCover(Cover remoteCover)
        {
            return FileConductorViewModel.SaveCover(remoteCover)
                .Do(x => { }, ex => { CoverRetrieverErrorMessage = ex.Message; })
                .OnErrorResumeNext(Observable.Empty<Unit>());
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

        /// <summary>
        /// Determines whether this instance can execute save cover command.
        /// </summary>
        /// <param name="remoteCover">The remote cover.</param>
        /// <returns>
        ///   <c>true</c> if this instance can execute save cover command; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteSaveCoverCommand(RemoteCover remoteCover)
        {
            return String.IsNullOrEmpty(CoverRetrieverErrorMessage) && SuggestedCovers.Count > 0;
        }

        /// <summary>
        /// Suggested covers on collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="notifyCollectionChangedEventArgs">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void SuggestedCoversOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            saveCoverCommand.RaiseCanExecuteChanged();
        }
  }
}