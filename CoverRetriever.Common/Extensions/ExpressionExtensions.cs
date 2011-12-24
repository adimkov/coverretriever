// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionExtensions.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Linq expressions extension
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Common.Extensions
{
    using System;

    /// <summary>
    /// Linq expressions extension.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Withes the specified value.
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="action">The action.</param>
        /// <returns>The value</returns>
        public static T With<T>(this T value, Action<T> action) 
            where T : class
        {
            if (value != null)
            {
                action(value);
            }

            return value;
        }

        /// <summary>
        /// Withes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="action">The action.</param>
        /// <returns>The value</returns>
        public static string With(this string value, Action<string> action) 
        {
            if (!String.IsNullOrEmpty(value))
            {
                action(value);
            }

            return value;
        }
    }
}