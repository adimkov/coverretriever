using System;
using System.Globalization;
using System.Windows.Data;
using CoverRetriever.Model;

namespace CoverRetriever.Converter
{
    /// <summary>
    /// Convert  <see cref="CoverRecipient"/> into <see cref="bool"/>.
    /// <remarks>
    /// CoverRecipient is true   
    /// </remarks>
    /// </summary>
    public class CoverRecipientToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((CoverRecipient) value) == CoverRecipient.Frame;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool) value) ? CoverRecipient.Frame : CoverRecipient.Directory;
        }
    }
}