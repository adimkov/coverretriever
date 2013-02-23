// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileConductorViewModel.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  File conductor view model
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Windows.Input;

    using CoverRetriever.AudioInfo;
    using CoverRetriever.AudioInfo.Tagger;
    using CoverRetriever.Model;
    using CoverRetriever.Resources;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

    using Notification = Microsoft.Practices.Prism.Interactivity.InteractionRequest.Notification;

    /// <summary>
    /// View model of selected audio file conductor. 
    /// </summary>
    [Export]
    public class FileConductorViewModel : ViewModelBase
    {
        /// <summary>
        /// The request to highlight GetTag button.
        /// </summary>
        private readonly InteractionRequest<Notification> highlightToGetTags;

        /// <summary>
        /// Backing field for GrabTagsCommand. 
        /// </summary>
        private readonly DelegateCommand grabTagsCommand;

        /// <summary>
        /// Get covers subject.
        /// </summary>
        private readonly Subject<Tuple<bool, IMetaProvider>> grabCoverSubject = new Subject<Tuple<bool, IMetaProvider>>();

        /// <summary>
        /// Backing field for SelectedAudio property.
        /// </summary>
        private AudioFile selectedAudio;

        /// <summary>
        /// Backing field for Recipient property.
        /// </summary>
        private CoverRecipient recipient;

        /// <summary>
        /// Backing field for SelectedAudioCover property.
        /// </summary>
        private Cover selectedAudioCover;

        /// <summary>
        /// Backing field for ApplyToAllFiles property.
        /// </summary>
        private bool applyToAllFiles;

        /// <summary>
        /// Backing field for Save
        /// </summary>
        private bool saveTagsOptionsIsOpened;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileConductorViewModel" /> class.
        /// </summary>
        public FileConductorViewModel()
        {
            highlightToGetTags = new InteractionRequest<Notification>();
            grabTagsCommand = new DelegateCommand(GrabTagsCommandExecute, () => SelectedAudio != null);
            SaveSuggestedTagCommand = new DelegateCommand(SaveSuggestedTagCommandExecute);
            SaveSuggestedTagsInContextCommand = new DelegateCommand(SaveSuggestedTagsInContextCommandExecute);
            RejectSuggestedTagCommand = new DelegateCommand(RejectSuggestedTagCommandExecute);
            LoadCaversCommand = new DelegateCommand(LoadCaversCommandExecute);

            grabCoverSubject
                .Throttle(TimeSpan.FromMilliseconds(200))
                .ObserveOnDispatcher()
                .Subscribe(GrabCoversForAudio);
        }

        /// <summary>
        /// Gets the grab tags command.
        /// </summary>
        public ICommand GrabTagsCommand
        {
            get
            {
                return grabTagsCommand;
            }
        }

        /// <summary>
        /// Gets the reject suggested tags command.
        /// </summary>
        public ICommand SaveSuggestedTagCommand { get; private set; }

        /// <summary>
        /// Gets the reject suggested tags command.
        /// </summary>
        public ICommand RejectSuggestedTagCommand { get; private set; }

        /// <summary>
        /// Gets the save suggested tags in context command.
        /// </summary>
        /// <value>
        /// The save suggested tags in context command.
        /// </value>
        public ICommand SaveSuggestedTagsInContextCommand { get; private set; }

        /// <summary>
        /// Gets the load cavers command.
        /// </summary>
        /// <value>
        /// The load cavers command.
        /// </value>
        public ICommand LoadCaversCommand { get; private set; }

        /// <summary>
        /// Gets or sets the audio tagger.
        /// </summary>
        /// <value>
        /// The audio tagger.
        /// </value>
        [Import(typeof(ITaggerService))]
        public Lazy<ITaggerService> Tagger { get; set; }

        /// <summary>
        /// Gets or sets selected audio. 
        /// </summary>
        public virtual AudioFile SelectedAudio
        {
            get
            {
                return selectedAudio;
            }

            set
            {
                if (selectedAudio != null)
                {
                    selectedAudio.CancelEditTags();
                }
                                                   
                selectedAudio = value;
                if (selectedAudio != null)
                {
                    selectedAudio.BeginEditTags();
                }

                SetSelectedAudioCoverIfPosible();
                RaisePropertyChanged(string.Empty);
                grabTagsCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets <see cref="CoverRecipient">cover</see> from audio by selected <see cref="CoverRecipient">recipient</see>.
        /// </summary>
        public Cover SelectedAudioCover
        {
            get
            {
                return selectedAudioCover;
            }

            private set
            {
                selectedAudioCover = value;
                RaisePropertyChanged("SelectedAudioCover");
            }
        }

        /// <summary>
        /// Gets or sets the artist of composition.
        /// </summary>
        public string Artist
        {
            get
            {
                return SelectedAudio != null ? SelectedAudio.MetaProvider.Artist : string.Empty;
            }

            set
            {
                if (SelectedAudio != null)
                {
                    SelectedAudio.MetaProvider.Artist = value;
                    RaisePropertyChanged("IsDirty");
                }
            }
        }

        /// <summary>
        /// Gets or sets album name.
        /// </summary>
        public string Album
        {
            get
            {
                return SelectedAudio != null ? SelectedAudio.MetaProvider.Album : string.Empty;
            }

            set
            {
                if (SelectedAudio != null)
                {
                    SelectedAudio.MetaProvider.Album = value;
                    RaisePropertyChanged("IsDirty");
                }
            }
        }

        /// <summary>
        /// Gets or sets of year.
        /// </summary>
        public string Year
        {
            get
            {
                return SelectedAudio != null ? SelectedAudio.MetaProvider.Year : string.Empty;
            }

            set
            {
                if (SelectedAudio != null)
                {
                    SelectedAudio.MetaProvider.Year = value;
                    RaisePropertyChanged("IsDirty");
                }
            }
        }

        /// <summary>
        /// Gets or sets name of composition.
        /// </summary>
        public string TrackName
        {
            get
            {
                return SelectedAudio != null ? SelectedAudio.MetaProvider.TrackName : string.Empty;
            }

            set
            {
                if (SelectedAudio != null)
                {
                    SelectedAudio.MetaProvider.TrackName = value;
                    RaisePropertyChanged("IsDirty");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirty
        {
            get
            {
                return SelectedAudio != null && SelectedAudio.MetaProvider.IsDirty;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether apply cover to current audio or to all in folder.
        /// </summary>
        public bool ApplyToAllFiles
        {
            get
            {
                return applyToAllFiles;
            }

            set
            {
                applyToAllFiles = value;
                RaisePropertyChanged("ApplyToAllFiles");
            }
        }

        /// <summary>
        /// Gets or sets the recipient.
        /// </summary>
        /// <value>The recipient.</value>
        public CoverRecipient Recipient
        {
            get
            {
                return recipient;
            }

            set
            {
                if (recipient != value)
                {
                    recipient = value;

                    if (recipient == CoverRecipient.Directory)
                    {
                        ApplyToAllFiles = false;
                    }

                    if (SelectedAudio != null)
                    {
                        SetSelectedAudioCoverIfPosible();
                    }

                    RaisePropertyChanged("Recipient");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is needed to retrieve tags.
        /// </summary>
        /// <value>
        ///     <c>true</c> If this instance is needed to retrieve tags; otherwise, <c>false</c>.
        /// </value>
        public bool IsNeededToRetrieveTags
        {
            get
            {
                return String.IsNullOrWhiteSpace(Artist);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether save tags options is opened.
        /// </summary>
        /// <value>
        /// <c>true</c> If save tags options is opened; otherwise, <c>false</c>.
        /// </value>
        public bool SaveTagsOptionsIsOpened
        {
            get
            {
                return saveTagsOptionsIsOpened;
            }

            set
            {
                saveTagsOptionsIsOpened = value;
                RaisePropertyChanged("SaveTagsOptionsIsOpened");
            }
        }

        /// <summary>
        /// Gets the request for highlight 'get tags' button.
        /// </summary>
        public InteractionRequest<Notification> HighlightToGetTags
        {
            get
            {
                return highlightToGetTags;
            }
        }

        /// <summary>
        /// Saves the selected cover onto disk.
        /// </summary>
        /// <param name="remoteCover">The remote cover.</param>
        /// <returns>Save cover operation indicator.</returns>
        public virtual IObservable<Unit> SaveCover(Cover remoteCover)
        {
            if (!ApplyToAllFiles)
            {
                var recipient = GrabCoverOrganizer(SelectedAudio);
                return recipient
                    .SaveCover(remoteCover)
                    .Finally(SetSelectedAudioCoverIfPosible);
            }
            
            if (Recipient == CoverRecipient.Frame)
            {
                var cachedCover = new CachedCover(remoteCover);

                return GrabCoverOrganizerInCurrentFolder()
                    .Select(x => x.SaveCover(cachedCover))
                    .Merge()
                    .Finally(SetSelectedAudioCoverIfPosible);
            }

            throw new InvariantException("Attempt to save covers in {0} and apply to all", Recipient);
        }

        /// <summary>
        /// Get cover from audio by recipient.
        /// </summary>
        /// <param name="audio">Audio file to retrieve cover organizer.</param>
        /// <returns>Cover organizer.</returns>
        private ICoverOrganizer GrabCoverOrganizer(AudioFile audio)
        {
            return Recipient == CoverRecipient.Directory ? audio.DirectoryCover : audio.FrameCover;
        }

        /// <summary>
        /// Get covers from audio by recipient.
        /// </summary>
        /// <returns>Covers collection.</returns>
        private IEnumerable<ICoverOrganizer> GrabCoverOrganizerInCurrentFolder()
        {
            var recipients = Enumerable.Empty<ICoverOrganizer>();

            var parentFolder = SelectedAudio.Parent as Folder;
            if (parentFolder != null)
            {
                recipients = parentFolder.Children
                    .OfType<AudioFile>()
                    .Select(GrabCoverOrganizer)
                    .ToArray(); // copy data in memory
            }

            return recipients;
        }

        /// <summary>
        /// Get a cover if it exists. Otherwise set null.
        /// </summary>
        private void SetSelectedAudioCoverIfPosible()
        {
            Cover cover = null;
            if (SelectedAudio != null)
            {
                var recipient = GrabCoverOrganizer(SelectedAudio);
                cover = recipient.IsCoverExists() ? recipient.GetCover() : null;
            }

            SelectedAudioCover = cover;
        }

        /// <summary>
        /// Grabs the tags command execute.
        /// </summary>
        private void GrabTagsCommandExecute()
        {
            StartOperation(CoverRetrieverResources.GrabTagMessage.FormatString(SelectedAudio.Name));

            Tagger.Value.LoadTagsForAudioFile(SelectedAudio.GetFileSystemItemFullPath())
                .SubscribeOn(((CoverRetrieverViewModel)ParentViewModel).SubscribeScheduler)
                .ObserveOn(((CoverRetrieverViewModel)ParentViewModel).ObservableScheduler)
                .Finally(EndOperation)
                .Completed(() =>
                    {
                        Trace.TraceInformation("Tags received for {0}", SelectedAudio.Name);
                        grabCoverSubject.OnNext(Tuple.Create(false, SelectedAudio.MetaProvider));
                    })
                .Subscribe(
                    x =>
                    {
                        SelectedAudio.CopyTagsFrom(x);
                        RaisePropertyChanged(string.Empty);
                    },
                    ((CoverRetrieverViewModel)ParentViewModel).SetError);
        }

        /// <summary>
        /// Rejects the suggested tag command execute.
        /// </summary>
        private void SaveSuggestedTagCommandExecute()
        {
            SelectedAudio.SaveFromTagger();
            SelectedAudio.EndEditTags();
            SelectedAudio.BeginEditTags();
            RaisePropertyChanged(string.Empty);
        }

        /// <summary>
        /// Saves the suggested tags in context command execute.
        /// </summary>
        private void SaveSuggestedTagsInContextCommandExecute()
        {
            var parentFolder = SelectedAudio.Parent as Folder;
            var tag = SelectedAudio.MetaProvider;
            SaveTagsOptionsIsOpened = false;
            if (parentFolder != null)
            {
                var audioFiles = parentFolder.Children
                    .OfType<AudioFile>()
                    .ToArray(); // copy data in memory

                foreach (var audioFile in audioFiles)
                {
                    StartOperation(CoverRetrieverResources.MessageSaveTagsInContext.FormatString(audioFile.Name));
                    
                    audioFile.CopyTagsFrom(tag);
                    //audioFile.SaveFromTagger();
                }

                EndOperation();
            }
        }

        /// <summary>
        /// Rejects the suggested tag command execute.
        /// </summary>
        private void RejectSuggestedTagCommandExecute()
        {
            SelectedAudio.CancelEditTags();
            grabCoverSubject.OnNext(Tuple.Create(true, SelectedAudio.MetaProvider)); 
            SelectedAudio.BeginEditTags();
            RaisePropertyChanged(string.Empty);
        }

        /// <summary>
        /// Loads the cavers command execute.
        /// </summary>
        private void LoadCaversCommandExecute()
        {
            grabCoverSubject.OnNext(Tuple.Create(false, SelectedAudio.MetaProvider)); 
        }

        /// <summary>
        /// Grabs the covers for audio.
        /// </summary>
        /// <param name="metaProvider">The meta provider.</param>
        private void GrabCoversForAudio(Tuple<bool, IMetaProvider> metaProvider)
        {
            if (metaProvider.Item1 || metaProvider.Item2.IsDirty)
            {
                ((CoverRetrieverViewModel)ParentViewModel).FindRemoteCovers(metaProvider.Item2);
            }
        }
    }
}