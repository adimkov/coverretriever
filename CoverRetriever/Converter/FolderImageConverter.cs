using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media.Imaging;

using CoverRetriever.Infrastructure;
using CoverRetriever.Model;

namespace CoverRetriever.Converter
{
	public class FolderImageConverter : IValueConverter
	{
		private const string ShellFolder = "/CoverRetriever;component/Assets/ShellFolder.png";
		private const string ShellAudioFolder = "/CoverRetriever;component/Assets/ShellFolder_Audio.png";

		#region Implementation of IValueConverter

		/// <summary>
		/// Converts a value. 
		/// </summary>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		/// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var folder = value as Folder;
			var resultPath = ShellFolder;
			if (folder != null && folder.Children.Any(x => AudioFormat.AudioFileExtensions.Any(ext => ext == Path.GetExtension(x.Name).ToLower())))
			{
				resultPath = ShellAudioFolder;
			}

			return new BitmapImage(new Uri(resultPath, UriKind.Relative));
		}

		/// <summary>
		/// Converts a value. 
		/// </summary>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		/// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}