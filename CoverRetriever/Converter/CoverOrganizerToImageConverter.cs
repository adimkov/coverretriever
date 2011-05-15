using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using CoverRetriever.Service;

namespace CoverRetriever.Converter
{
	public class CoverOrganizerToImageConverter : IValueConverter
	{
		private readonly Uri NoCoverUri = new Uri("/CoverRetriever;component/Assets/Shell_NoCover.png", UriKind.Relative);
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
			var directoryCoverOrganizer = (ICoverOrganizer) value;
			if (directoryCoverOrganizer.IsCoverExists())
			{
				return directoryCoverOrganizer.GetCover().CoverStream;
			}
			var info = Application.GetResourceStream(NoCoverUri);
			return Observable.Return(info.Stream);
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