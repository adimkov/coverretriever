using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using CoverRetriever.AudioInfo;
using CoverRetriever.Model;

namespace CoverRetriever.ViewModel
{
    /// <summary>
    /// View model of selected audio file conductor 
    /// </summary>
    [Export]
    public class FileConductorViewModel : ViewModelBase
    {
        private AudioFile _selectedAudio;
        private CoverRecipient _recipient;
        private Cover _selectedAudioCover;
        private bool _applyToAllFiles;

        /// <summary>
        /// Get or set SelectedAudio 
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
            }
        }

        /// <summary>
        /// Get <see cref="CoverRecipient">cover</see> from audio by selected <see cref="CoverRecipient">recipient</see>
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
        /// Gets or sets a value indicating is apply cover to current audio or to all in folder
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
        /// Get or set recipient of cover
        /// </summary>
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
                    if (SelectedAudio != null)
                    {
                        SetSelectedAudioCoverIfPosible();
                    }
                    RaisePropertyChanged("Recipient");
                }
            }
        }

        /// <summary>
        /// perform save cover
        /// </summary>
        /// <param name="remoteCover"></param>
        /// <returns></returns>
        public virtual IObservable<Unit> SaveCover(RemoteCover remoteCover)
        {
            if (!ApplyToAllFiles)
            {
                var recipient = GrabCoverOrganizer(SelectedAudio);
                return recipient
                    .SaveCover(remoteCover)
                    .Finally(() => SetSelectedAudioCoverIfPosible());
            }
            
            if (Recipient == CoverRecipient.Frame)
            {
                // todo: replace to Merge
                return Observable.Concat(
                    GrabCoverOrganizerInCurrentFolder()
                        .Select(x => x.SaveCover(remoteCover)))      
                        .Finally(() => SetSelectedAudioCoverIfPosible());
            }

            throw new ModelInvalidStateException("Attempt to save covers in {0} and apply to all", Recipient);
        }

        /// <summary>
        /// Get cover from audio by recipient
        /// </summary>
        /// <param name="audio">Audio file to retrieve cover organizer</param>
        /// <returns>Cover organizer</returns>
        private ICoverOrganizer GrabCoverOrganizer(AudioFile audio)
        {
            return Recipient == CoverRecipient.Directory ? audio.DirectoryCover : audio.FrameCover;
        }

        /// <summary>
        /// Get covers from audio by recipient
        /// </summary>
        /// <returns>Covers collection</returns>
        private IEnumerable<ICoverOrganizer> GrabCoverOrganizerInCurrentFolder()
        {
            var recipients = Enumerable.Empty<ICoverOrganizer>();

            var parentFolder = SelectedAudio.Parent as Folder;
            if (parentFolder != null)
            {
                recipients = parentFolder.Children
                    .OfType<AudioFile>()
                    .Select(GrabCoverOrganizer)
                    .ToArray();//copy data in memory
            }

            return recipients;
        }

        /// <summary>
        /// Get a cover if it exists. Otherwise set null
        /// </summary>
        private void SetSelectedAudioCoverIfPosible()
        {
            if (SelectedAudio != null)
            {
                var recipient = GrabCoverOrganizer(SelectedAudio);
                SelectedAudioCover = recipient.IsCoverExists() ? recipient.GetCover() : null;
            }
        }
    }
}