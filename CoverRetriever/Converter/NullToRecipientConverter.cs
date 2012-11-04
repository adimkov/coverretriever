// -----------------------------------------------------------------------------------------------
// <copyright file="NullToRecipientConverter.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// -----------------------------------------------------------------------------------------------

namespace CoverRetriever.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Declaration of the <see cref="NullToRecipientConverter{TInput}" /> converter.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    public class NullToRecipientConverter<TInput> : IValueConverter
    {
        /// <summary>
        /// Gets or sets the <c>true</c>value.
        /// </summary>
        /// <value>
        /// The true value.
        /// </value>
        public TInput TrueValue { get; set; }

        /// <summary>
        /// Gets or sets the <c>false</c> value.
        /// </summary>
        /// <value>
        /// The false value.
        /// </value>
        public TInput FalseValue { get; set; }

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
            return value != null ? TrueValue : FalseValue;
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
            return TrueValue.Equals(value);
        }

        #endregion
    }
}