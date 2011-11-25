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
    using System.Linq;

    using CoverRetriever.AudioInfo;
    using CoverRetriever.Model;

    /// <summary>
    /// View model of selected audio file conductor. 
    /// </summary>
    [Export]
    public class FileConductorViewModel : ViewModelBase
    {
        /// <summary>
        /// Backing field for SelectedAudio property.
        /// </summary>
        private AudioFile _selectedAudio;

        /// <summary>
        /// Backing field for Recipient property.
        /// </summary>
        private CoverRecipient _recipient;

        /// <summary>
        /// Backing field for SelectedAudioCover property.
        /// </summary>
        private Cover _selectedAudioCover;

        /// <summary>
        /// Backing field for ApplyToAllFiles property.
        /// </summary>
        private bool _applyToAllFiles;

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
                    if (SelectedAudio != null)
                    {
                        SetSelectedAudioCoverIfPosible();
                    }

                    RaisePropertyChanged("Recipient");
                }
            }
        }

        /// <summary>
        /// Saves the selected cover onto disk.
        /// </summary>
        /// <param name="remoteCover">The remote cover.</param>
        /// <returns>Save cover operation indicator.</returns>
        public virtual IObservable<Unit> SaveCover(RemoteCover remoteCover)
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
    }
}