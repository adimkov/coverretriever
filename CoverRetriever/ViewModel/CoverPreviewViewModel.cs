using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

using CoverRetriever.Model;
using CoverRetriever.Resources;

using Microsoft.Practices.Prism.Commands;

namespace CoverRetriever.ViewModel
{
	public class CoverPreviewViewModel : ViewModelBase
	{
		private readonly RemoteCover _remoteCover;
		private readonly Subject<RemoteCover> _saveCoverSubject = new Subject<RemoteCover>();
		private readonly BitmapImage _coverImage;

		public CoverPreviewViewModel(RemoteCover remoteCover)
		{
			_remoteCover = remoteCover;
			StartOperation(CoverRetrieverResources.MessageCoverPreview);
			_coverImage = new BitmapImage(remoteCover.CoverUri);
			_coverImage.DownloadCompleted += CoverImageOnDownloadCompleted;
			_coverImage.DownloadFailed += CoverImageOnDownloadCompleted;

			SaveCoverCommand = new DelegateCommand(SaveCoverCommandExecute);
			FinishCommand = new DelegateCommand(FinishCommandExecute);
		}

		/// <summary>
		/// Get cover size
		/// </summary>
		public Size CoverSize
		{
			get
			{
				return _remoteCover.CoverSize;
			}
		}

		/// <summary>
		/// Get cover uri
		/// </summary>
		public BitmapImage CoverImage
		{
			get
			{
				return _coverImage;
			}
		}

		/// <summary>
		/// Perform cover save
		/// </summary>
		public DelegateCommand SaveCoverCommand { get; private set; }

		/// <summary>
		/// Release all resources
		/// </summary>
		public DelegateCommand FinishCommand { get; private set; }

		/// <summary>
		/// fire on user click Save cover button
		/// </summary>
		public IObservable<RemoteCover> SaveCover
		{
			get
			{
				return _saveCoverSubject;
			}
		}

		/// <summary>
		/// Set busy indicator state
		/// </summary>
		/// <param name="isBusy"></param>
		/// <param name="operationMethod"></param>
		public void SetBusy(bool isBusy, string operationName)
		{
			if (isBusy)
			{
				StartOperation(operationName);
			}
			else
			{
				EndOperation();
			}
		}

		private void CoverImageOnDownloadCompleted(object sender, EventArgs eventArgs)
		{
			EndOperation();
		}

		private void SaveCoverCommandExecute()
		{
			_saveCoverSubject.OnNext(_remoteCover);
		}

		private void FinishCommandExecute()
		{
			_saveCoverSubject.OnCompleted();
		}
	}
}