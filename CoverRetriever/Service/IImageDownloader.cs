using System;
using System.Windows.Media.Imaging;

namespace CoverRetriever.Service
{
	public interface IImageDownloader
	{
		/// <summary>
		/// Downloaded image
		/// </summary>
		BitmapImage BitmapImage { get; }

		/// <summary>
		/// Download an image from web
		/// </summary>
		/// <param name="imageUri">image uri</param>
		/// <returns>Download Progress</returns>
		IObservable<double> DownloadImage(Uri imageUri);
	}
}