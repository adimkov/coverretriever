using System;
using System.Globalization;
using System.Windows.Data;

using CoverRetriever.Model;

namespace CoverRetriever.Converter
{
	public class FolderSourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var folder = value as Folder;

			if (folder != null)
			{
				return folder.Children;
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}