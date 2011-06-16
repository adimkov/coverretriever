using System;
using System.ComponentModel.Composition;
using System.Linq;
using CoverRetriever.Model;

namespace CoverRetriever.ViewModel
{
	[Export]
	public class FileConductorViewModel : ViewModelBase
	{
		private AudioFile _selectedAudio;
		private CoverRecipient _recipient;
		
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
				RaisePropertyChanged("SelectedAudio");
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
				_recipient = value;
				RaisePropertyChanged("Recipient");
			}
		}

		/// <summary>
		/// perform save cover
		/// </summary>
		/// <param name="remoteCover"></param>
		/// <returns></returns>
		public virtual IObservable<Unit> SaveCover(RemoteCover remoteCover)
		{
			var recipient = Recipient == CoverRecipient.Directory ? SelectedAudio.DirectoryCover : SelectedAudio.FrameCover;
			return recipient
				.SaveCover(remoteCover)
				.Do(
					x =>
					{
						var swapFileDetails = SelectedAudio;
						SelectedAudio = null;
						SelectedAudio = swapFileDetails;
					});
		}
	}
}