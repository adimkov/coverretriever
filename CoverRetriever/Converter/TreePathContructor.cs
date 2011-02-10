using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

using CoverRetriever.Model;

namespace CoverRetriever.Converter
{
	public class TreePathContructor : IValueConverter
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
			if(value != null)
			{
				return GetObjectPath((FileSystemItem)value).Reverse();
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

		private static IEnumerable<FileSystemItem> GetObjectPath(FileSystemItem item)
		{
			var list = new List<FileSystemItem>();
			list.Add(item);
			if (item.Parent != null && !(item.Parent is RootFolder))
			{
				list.AddRange(GetObjectPath(item.Parent));
			}

			return list;
		}
	}
}