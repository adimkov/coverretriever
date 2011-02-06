using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media.Imaging;

using CoverRetriever.Service;

namespace CoverRetriever.Converter
{
	public class CoverOrganizerToImageConverter : IValueConverter
	{
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
			var coverOrganizerList = (IEnumerable<ICoverOrganizer>)value;
			var directoryCoverOrganizer = coverOrganizerList.First();
			if (directoryCoverOrganizer.IsCoverExists())
			{
				//read cover in memory and release file
				using (var coverStream = File.OpenRead(directoryCoverOrganizer.GetCoverFullPath()))
				{
					var ms = new MemoryStream();

					ms.SetLength(coverStream.Length);
					coverStream.Read(ms.GetBuffer(), 0, (int)coverStream.Length);
					ms.Flush();
					
					BitmapImage src = new BitmapImage();
					src.BeginInit();
					src.StreamSource = ms;
					src.EndInit();

					return src;
				}
			}
			return null;
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