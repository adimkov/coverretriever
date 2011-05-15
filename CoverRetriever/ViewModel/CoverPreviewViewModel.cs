using System;
using System.Collections.Generic;
using System.Linq;
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
		private string _errorMessage;
		private double _downloadProgress;
		private BitmapImage _coverImage;

		public CoverPreviewViewModel(RemoteCover remoteCover)
		{
			_remoteCover = remoteCover;
			StartOperation(CoverRetrieverResources.MessageCoverPreview);
			SaveCoverCommand = new DelegateCommand(SaveCoverCommandExecute, () => String.IsNullOrEmpty(ErrorMessage));
			CloseCommand = new DelegateCommand(CloseCommandExecute);
			
			remoteCover.CoverStream
				.Finally(EndOperation)
				.Subscribe(
					stream =>
					{
						var coverImage = new BitmapImage();
						coverImage.BeginInit();
						coverImage.StreamSource = stream;
						coverImage.EndInit();
						CoverImage = coverImage;
					},
					ex =>
					{
						ErrorMessage = ex.Message;
					},
					() =>
					{
						ErrorMessage = null;
					});
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
		/// Get cover Image
		/// </summary>
		public BitmapImage CoverImage
		{
			get
			{
				return _coverImage;
			}
			private set
			{
				_coverImage = value;
				RaisePropertyChanged("CoverImage");
			}
		}

		/// <summary>
		/// Get reason of error
		/// </summary>
		public string ErrorMessage
		{
			get
			{
				return _errorMessage;
			}
			private set
			{
				_errorMessage = value;
				SaveCoverCommand.RaiseCanExecuteChanged();
				RaisePropertyChanged("ErrorMessage");
			}
		}
		
		/// <summary>
		/// Indicate image download progress
		/// <remarks>
		///	Property does not work at this moment
		/// </remarks>
		/// </summary>
		public double DownloadProgress
		{
			get
			{
				return _downloadProgress;
			}
			private set
			{
				_downloadProgress = value;
				RaisePropertyChanged("DownloadProgress");
			}
		}

		/// <summary>
		/// Perform cover save
		/// </summary>
		public DelegateCommand SaveCoverCommand { get; private set; }

		/// <summary>
		/// Release all resources
		/// </summary>
		public DelegateCommand CloseCommand { get; private set; }

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

		private void SaveCoverCommandExecute()
		{
			_saveCoverSubject.OnNext(_remoteCover);
		}

		private void CloseCommandExecute()
		{
			_saveCoverSubject.OnCompleted();
		}
	}
}