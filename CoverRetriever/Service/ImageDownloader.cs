using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CoverRetriever.Service
{
	[Export(typeof(IImageDownloader))]
	public class ImageDownloader : IImageDownloader
	{
		private readonly Subject<double> _downloadedSubject = new Subject<double>();
		/// <summary>
		/// Downloaded image
		/// </summary>
		public BitmapImage BitmapImage
		{
			get; private set;
		}

		/// <summary>
		/// Download an image from web
		/// </summary>
		/// <param name="imageUri">image uri</param>
		/// <returns>Download Progress</returns>
		public IObservable<double> DownloadImage(Uri imageUri)
		{
			BitmapImage = new BitmapImage(imageUri);
			BitmapImage.DownloadCompleted += BitmapImageOnDownloadCompleted;
			BitmapImage.DownloadFailed += BitmapImageOnDownloadFailed;
			BitmapImage.DownloadProgress += BitmapImageOnDownloadProgress;
			return _downloadedSubject;
		}

		private void BitmapImageOnDownloadCompleted(object sender, EventArgs eventArgs)
		{
			_downloadedSubject.OnCompleted();
			UnsubscribleFromEvents();
		}

		private void BitmapImageOnDownloadFailed(object sender, ExceptionEventArgs exceptionEventArgs)
		{
			_downloadedSubject.OnError(exceptionEventArgs.ErrorException);
			UnsubscribleFromEvents();
		}

		private void BitmapImageOnDownloadProgress(object sender, DownloadProgressEventArgs downloadProgressEventArgs)
		{
			_downloadedSubject.OnNext(downloadProgressEventArgs.Progress);	
		}

		private void UnsubscribleFromEvents()
		{
			BitmapImage.DownloadCompleted -= BitmapImageOnDownloadCompleted;
			BitmapImage.DownloadFailed -= BitmapImageOnDownloadFailed;
			BitmapImage.DownloadProgress -= BitmapImageOnDownloadProgress;
		}
	}
}