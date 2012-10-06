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
        /// Backing field for SelectedAudio property.
        /// </summary>
        private AudioFile _selectedAudio;

        /// <summary>
        /// Backing field for Recipient property.
        /// </summary>
        private CoverRecipient _recipient;

        /// <summary>
        /// Backing field for GrabTagsCommand 
        /// </summary>
        private DelegateCommand grabTagsCommand;

        /// <summary>
        /// Backing field for SelectedAudioCover property.
        /// </summary>
        private Cover _selectedAudioCover;

        /// <summary>
        /// Backing field for ApplyToAllFiles property.
        /// </summary>
        private bool _applyToAllFiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileConductorViewModel" /> class.
        /// </summary>
        public FileConductorViewModel()
        {
            highlightToGetTags = new InteractionRequest<Notification>();
            grabTagsCommand = new DelegateCommand(GrabTagsCommandExecute, () => SelectedAudio != null);
            SaveSuggestedTagCommand = new DelegateCommand(SaveSuggestedTagCommandExecute);
            RejectSuggestedTagCommand = new DelegateCommand(RejectSuggestedTagCommandExecute);
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
                return _selectedAudio;
            }

            set
            {
                _selectedAudio = value;
                SetSelectedAudioCoverIfPosible();
                RaisePropertyChanged("SelectedAudio");
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
                return _selectedAudioCover;
            }

            private set
            {
                _selectedAudioCover = value;
                RaisePropertyChanged("SelectedAudioCover");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether apply cover to current audio or to all in folder.
        /// </summary>
        public bool ApplyToAllFiles
        {
            get
            {
                return _applyToAllFiles;
            }

            set
            {
                _applyToAllFiles = value;
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
                return _recipient;
            }

            set
            {
                if (_recipient != value)
                {
                    _recipient = value;

                    if (_recipient == CoverRecipient.Directory)
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
        /// Gets or sets the parent view model.
        /// </summary>
        /// <value>
        /// The parent view model.
        /// </value>
        public CoverRetrieverViewModel ParentViewModel { get; set; }

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
                .SubscribeOn(ParentViewModel.SubscribeScheduler)
                .ObserveOn(ParentViewModel.ObservableScheduler)
                .Finally(EndOperation)
                .Completed(
                    () =>
                    {
                        ParentViewModel.FindRemoteCovers(SelectedAudio.MetaProvider);
                        Trace.TraceInformation("Tags received for {0}", SelectedAudio.Name);
                    })
                .Subscribe(
                    SelectedAudio.CopyTagsFrom,
                    ParentViewModel.SetError);
        }

        /// <summary>
        /// Rejects the suggested tag command execute.
        /// </summary>
        private void SaveSuggestedTagCommandExecute()
        {
            SelectedAudio.SaveFromTagger();
        }

        /// <summary>
        /// Rejects the suggested tag command execute.
        /// </summary>
        private void RejectSuggestedTagCommandExecute()
        {
            SelectedAudio.ResetTagger();
            ParentViewModel.FindRemoteCovers(SelectedAudio.MetaProvider);
        }
    }
}