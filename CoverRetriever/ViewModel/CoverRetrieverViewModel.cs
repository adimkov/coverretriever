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
    using System.Concurrency;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Input;

    using CoverRetriever.AudioInfo;
    using CoverRetriever.AudioInfo.Tagger;
    using CoverRetriever.Common.Interaction;
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
        private readonly IFileSystemService _fileSystemService;

        /// <summary>
        /// Service to grab album art from web.
        /// </summary>
        private readonly ICoverRetrieverService _coverRetrieverService;

        /// <summary>
        /// Backing field for Loaded command.
        /// </summary>
        private readonly DelegateCommand _loadedCommand;

        /// <summary>
        /// Backing field for FileSystemSelectedItemChanged command.
        /// </summary>
        private readonly DelegateCommand<FileSystemItem> _fileSystemSelectedItemChangedCommand;

        /// <summary>
        /// Backing field for PreviewCommand command.
        /// </summary>
        private readonly DelegateCommand<RemoteCover> _previewCoverCommand;

        /// <summary>
        /// Backing field for SaveCover command.
        /// </summary>
        private readonly DelegateCommand<RemoteCover> _saveCoverCommand;

        /// <summary>
        /// Backing field for SelectFolder command.
        /// </summary>
        private readonly DelegateCommand _selectFolderCommand;

        /// <summary>
        /// Backing field for Finish command.
        /// </summary>
        private readonly DelegateCommand _finishCommand;

        /// <summary>
        /// Backing field for About command.
        /// </summary>
        private readonly DelegateCommand _aboutCommand;
        
        /// <summary>
        /// Backing field for CloseError command.
        /// </summary>
        private readonly DelegateCommand _closeErrorMessage;

        /// <summary>
        /// Backing field for GrabTags command.
        /// </summary>
        private readonly DelegateCommand _grabTagsCommand;

        /// <summary>
        /// Backing field for RejectSuggestedTag command.
        /// </summary>
        private readonly DelegateCommand _rejectSuggestedTagCommand;

        /// <summary>
        /// Backing field for SaveSuggestedTag command.
        /// </summary>
        private readonly DelegateCommand _saveSuggestedTagCommand;

        /// <summary>
        /// The request to enlarge selected cover.
        /// </summary>
        private readonly InteractionRequest<Notification> _previewCoverRequest;

        /// <summary>
        /// The request to open library window.
        /// </summary>
        private readonly InteractionRequest<Notification> _selectRootFolderRequest;

        /// <summary>
        /// The request to highlight GetTag button.
        /// </summary>
        private readonly InteractionRequest<Notification> _highlightToGetTags;

        /// <summary>
        /// The request to show about window.
        /// </summary>
        private readonly InteractionRequest<Notification> _aboutRequest;

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
        private string _coverRetrieverErrorMessage;

        /// <summary>
        /// New version of application.
        /// </summary>
        private string _newVersion;

        /// <summary>
        /// Backing field for SaveTagMode property.
        /// </summary>
        private bool _saveTagMode;

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

            openFolderViewModel.PushRootFolder.Subscribe(OnNextPushRootFolder);
            _loadedCommand = new DelegateCommand(LoadedCommandExecute);
            _fileSystemSelectedItemChangedCommand = new DelegateCommand<FileSystemItem>(FileSystemSelectedItemChangedCommandExecute);
            _previewCoverCommand = new DelegateCommand<RemoteCover>(PreviewCoverCommandExecute);
            _saveCoverCommand = new DelegateCommand<RemoteCover>(SaveCoverCommandExecute, CanExecuteSaveCoverCommand);
            _selectFolderCommand = new DelegateCommand(SelectFolderCommandExecute);
            _finishCommand = new DelegateCommand(FinishCommandExecute);
            _aboutCommand = new DelegateCommand(AboutCommandExecute);
            _closeErrorMessage = new DelegateCommand(CloseErrorMessageExecute);
            _grabTagsCommand = new DelegateCommand(GrabTagsCommandExecute, () => FileConductorViewModel.SelectedAudio is AudioFile);
            _rejectSuggestedTagCommand = new DelegateCommand(RejectSuggestedTagCommandExecute);
            _saveSuggestedTagCommand = new DelegateCommand(SaveSuggestedTagCommandExecute);

            _previewCoverRequest = new InteractionRequest<Notification>();
            _selectRootFolderRequest = new InteractionRequest<Notification>();
            _highlightToGetTags = new InteractionRequest<Notification>();
            _aboutRequest = new InteractionRequest<Notification>();

            _suggestedCovers.CollectionChanged += SuggestedCoversOnCollectionChanged;

            ObservableScheduler = Scheduler.Dispatcher;
            SubscribeScheduler = Scheduler.ThreadPool;
        }

        /// <summary>
        /// Gets Loaded window Command.
        /// </summary>
        public ICommand LoadedCommand
        {
            get
            {
                return _loadedCommand;
            }
        }

        /// <summary>
        /// Gets selection changed in file System tree view command.
        /// </summary>
        public ICommand FileSystemSelectedItemChangedCommand
        {
            get
            {
                return _fileSystemSelectedItemChangedCommand;
            }
        }
        
        /// <summary>
        /// Gets preview selected cover.
        /// </summary>
        public ICommand PreviewCoverCommand       
        {
            get
            {
                return _previewCoverCommand;
            }
        }
        
        /// <summary>
        /// Gets save selected cover in to selected directory.
        /// </summary>
        public ICommand SaveCoverCommand
        {
            get
            {
                return _saveCoverCommand;
            }
        }

        /// <summary>
        /// Gets change root directory command.
        /// </summary>
        public ICommand SelectFolderCommand
        {
            get
            {
                return _selectFolderCommand;
            }
        }

        /// <summary>
        /// Gets finish work of application.
        /// </summary>
        public ICommand FinishCommand
        {
            get
            {
                return _finishCommand;
            }
        }
        
        /// <summary>
        /// Gets about dialog command.
        /// </summary>
        public ICommand AboutCommand
        {
            get
            {
                return _aboutCommand;
            }
        }

        /// <summary>
        /// Gets close an error message.
        /// </summary>
        public ICommand CloseErrorMessage
        {
            get
            {
                return _closeErrorMessage;
            }
        }

        /// <summary>
        /// Gets the grab tags command.
        /// </summary>
        public ICommand GrabTagsCommand
        {
            get
            {
                return _grabTagsCommand;
            }
        }

        /// <summary>
        /// Gets the reject suggested tags command.
        /// </summary>
        public ICommand RejectSuggestedTagCommand
        {
            get
            {
                return _rejectSuggestedTagCommand;
            }
        }

        /// <summary>
        /// Gets the reject suggested tags command.
        /// </summary>
        public ICommand SaveSuggestedTagCommand
        {
            get
            {
                return _saveSuggestedTagCommand;
            }
        }

        /// <summary>
        /// Gets preview cover dialog request.
        /// </summary>
        public IInteractionRequest PreviewCoverRequest
        {
            get
            {
                return _previewCoverRequest;
            }
        }
        
        /// <summary>
        /// Gets select new root folder dialog.
        /// </summary>
        public IInteractionRequest SelectRootFolderRequest
        {
            get
            {
                return _selectRootFolderRequest;
            }
        }

        /// <summary>
        /// Gets the request for highlight 'get tags' button.
        /// </summary>
        public IInteractionRequest HighlightToGetTags
        {
            get
            {
                return _highlightToGetTags;
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
                return _aboutRequest;
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
        public string CoverRetrieverErrorMessage
        {
            get
            {
                return _coverRetrieverErrorMessage;
            }

            private set
            {
                _coverRetrieverErrorMessage = value;
                _saveCoverCommand.RaiseCanExecuteChanged();
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
                return _newVersion;
            }

            private set
            {
                _newVersion = value;
                RaisePropertyChanged("NewVersion");
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
        /// Gets or sets a value indicating whether view in save tag mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if view in save tag mode; otherwise, <c>false</c>.
        /// </value>
        public bool SaveTagMode
        {
            get
            {
                return _saveTagMode;
            } 

            set
            {
                _saveTagMode = value;
                RaisePropertyChanged("SaveTagMode");
            }
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
        /// Gets or sets the audio tagger.
        /// </summary>
        /// <value>
        /// The audio tagger.
        /// </value>
        [Import(typeof(ITagger))]
        public Lazy<ITagger> Tagger { get; set; }

        /// <summary>
        /// Loads the command execute.
        /// </summary>
        private void LoadedCommandExecute()
        {
            if (_rootFolder == null)
            {
                _openFolderViewModel.IsCloseEnabled = false;
                _selectFolderCommand.Execute();
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
            StartOperation(CoverRetrieverResources.MessageLibraryLoad);
            _fileSystemService.GetChildrenForRootFolder(_rootFolder)
                .SubscribeOn(SubscribeScheduler)
                .ObserveOn(ObservableScheduler)
                .Subscribe(
                x => _rootFolder.Children.Add(x),
                SelectFirstAudioFile);
            _selectRootFolderRequest.Raise(new CloseNotification());
        }

        /// <summary>
        /// Selects the first audio file.
        /// </summary>
        private void SelectFirstAudioFile()
        {
            EndOperation();
            SelectedFileSystemItem = FindFirstAudioFile(_rootFolder);
        }

        /// <summary>
        /// Called when selected folder or audio file changed.
        /// </summary>
        /// <param name="file">The file or folder.</param>
        private void FileSystemSelectedItemChangedCommandExecute(FileSystemItem file)
        {
            SelectedFileSystemItem = file;
            SetAudioFile(file);
            if (FileConductorViewModel.SelectedAudio != null)
            {
                FileConductorViewModel.SelectedAudio.ResetTagger();
                SaveTagMode = false;
            }

            _grabTagsCommand.RaiseCanExecuteChanged();
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

            _previewCoverRequest.Raise(new Notification
            {
                Title = CoverRetrieverResources.CoverPreviewTitle.FormatUIString(
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
            _selectRootFolderRequest.Raise(new Notification
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
            _aboutRequest.Raise(new Notification
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
        /// Grabs the tags command execute.
        /// </summary>
        private void GrabTagsCommandExecute()
        {
            StartOperation(CoverRetrieverResources.GrabTagMessage.FormatString(SelectedFileSystemItem.Name));

            FileConductorViewModel.SelectedAudio.AssignTagger(Tagger.Value)
                .SubscribeOn(SubscribeScheduler)
                .ObserveOn(ObservableScheduler)
                .Finally(EndOperation)
                .Completed(
                () =>
                    {
                        FindRemoteCovers(FileConductorViewModel.SelectedAudio.MetaProvider);
                        SaveTagMode = true;
                        Trace.TraceInformation("Tags received for {0}", FileConductorViewModel.SelectedAudio.Name);
                    })
                .Subscribe(x => { }, SetError);
        }

        /// <summary>
        /// Rejects the suggested tag command execute.
        /// </summary>
        private void RejectSuggestedTagCommandExecute()
        {
            FileConductorViewModel.SelectedAudio.ResetTagger();
            SaveTagMode = false;
            FindRemoteCovers(FileConductorViewModel.SelectedAudio.MetaProvider);
        }

                /// <summary>
        /// Rejects the suggested tag command execute.
        /// </summary>
        private void SaveSuggestedTagCommandExecute()
        {
            FileConductorViewModel.SelectedAudio.SaveFromTagger();
            SaveTagMode = false;
        }

        /// <summary>
        /// Sets the error cover retrieve.
        /// </summary>
        /// <param name="ex">The exception.</param>
        private void SetError(Exception ex)
        {
            CoverRetrieverErrorMessage = ex.Message;
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
                if (audio.IsNeededToRetrieveTags)
                {
                    _highlightToGetTags.Raise(null);
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
        /// Perform search covers in web for audio.
        /// </summary>
        /// <param name="fileDetails">The audio file.</param>
        private void FindRemoteCovers(IMetaProvider fileDetails)
        {
            StartOperation(CoverRetrieverResources.MessageDownloadCover);
            ResetError();
            _suggestedCovers.Clear();
            var albumCondition = fileDetails.Album;

            _coverRetrieverService.GetCoverFor(fileDetails.Artist, albumCondition, SuggestedCountOfCovers)
                .SubscribeOn(SubscribeScheduler)
                .ObserveOn(ObservableScheduler)
                .Finally(EndOperation)
                .Subscribe(
                x =>
                {
                    SuggestedCovers.AddRange(x);
                    SelectedSuggestedCover = _suggestedCovers.Max();
                },
                SetError);
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
            _saveCoverCommand.RaiseCanExecuteChanged();
        }
  }
}