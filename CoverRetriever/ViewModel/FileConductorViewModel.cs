using System;
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
			var recipient = GrabCoverOrganizer();
			return recipient
				.SaveCover(remoteCover)
				.Do(
					x =>
					{
						SetSelectedAudioCoverIfPosible();
					});
		}

		/// <summary>
		/// Get cover from audio by recipient
		/// </summary>
		/// <returns></returns>
		private ICoverOrganizer GrabCoverOrganizer()
		{
			return Recipient == CoverRecipient.Directory ? SelectedAudio.DirectoryCover : SelectedAudio.FrameCover;
		}

		/// <summary>
		/// Get a cover if it exists. Otherwise set null
		/// </summary>
		private void SetSelectedAudioCoverIfPosible()
		{
			if (SelectedAudio != null)
			{
				var recipient = GrabCoverOrganizer();
				SelectedAudioCover = recipient.IsCoverExists() ? recipient.GetCover() : null;
			}
		}
	}
}