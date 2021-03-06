﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionExtensions.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Linq expressions extension
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace System.Linq
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
        /// <returns>The value.</returns>
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
        /// <returns>The value.</returns>
        public static string With(this string value, Action<string> action) 
        {
            if (!String.IsNullOrEmpty(value))
            {
                action(value);
            }

            return value;
        }

        /// <summary>
        /// Returns the specified value.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Value if it not null, otherwise - default value.</returns>
        public static T Return<T>(this T value, T defaultValue)
            where T : class
        {
            if (value != null)
            {
                return value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Returns the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Value if it not null, otherwise - default value.</returns>
        public static string Return(this string value, string defaultValue)
        {
            if (!String.IsNullOrEmpty(value))
            {
                return value;
            }

            return defaultValue;
        }

    }
}