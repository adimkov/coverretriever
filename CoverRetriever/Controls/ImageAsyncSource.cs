using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CoverRetriever.Controls
{
	public class ImageAsyncSource
	{
		public static readonly DependencyProperty AsyncSourceProperty =
			DependencyProperty.RegisterAttached("AsyncSource", typeof (IObservable<Stream>), typeof (ImageAsyncSource),
			                                    new PropertyMetadata(new PropertyChangedCallback(OnAsyncSourceChanged)));

		public static void SetAsyncSource(Image o, IObservable<Stream> value)
		{
			o.SetValue(AsyncSourceProperty, value);
		}

		public static IObservable<Stream> GetAsyncSource(DependencyObject o)
		{
			return (IObservable<Stream>) o.GetValue(AsyncSourceProperty);
		}

		private static void OnAsyncSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var image = d as Image;
			var source = e.NewValue as IObservable<Stream>;
			if (image != null && source != null)
			{
				image.Source = null;
				AddSource(image, source);
			}
		}

		private static void AddSource(Image image, IObservable<Stream> source)
		{
			source.Subscribe(stream =>
			                 	{
			                 		var bitmapImage = new BitmapImage();
									bitmapImage.BeginInit();
			                 		bitmapImage.StreamSource = stream;
			                 		bitmapImage.EndInit();

									image.Source = bitmapImage;
			                 	},
								ex =>
									{
										//muffle exception
										image.Source = null;
									});
		}
	}
}