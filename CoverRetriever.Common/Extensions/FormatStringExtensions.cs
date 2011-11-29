// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatStringExtensions.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Extension method to make deffered call.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;

namespace CoverRetriever.Common.Extensions
{
    /// <summary>
    /// Extensions of string
    /// </summary>
    public static class FormatStringExtensions
    {
        /// <summary>
        /// Invariant inline string format.
        /// </summary>
        /// <param name="str">String template.</param>
        /// <param name="param">The parameters.</param>
        /// <returns>Formatted string.</returns>
        public static string FormatString(this string str, params object[] param)
        {
            return String.Format(CultureInfo.InvariantCulture, str, param);
        }

        /// <summary>
        /// Inline string format.
        /// </summary>
        /// <param name="str">String template.</param>
        /// <param name="param">The parameters.</param>
        /// <returns>Formatted string.</returns>
        public static string FormatUIString(this string str, params object[] param)
        {
            return String.Format(CultureInfo.CurrentUICulture, str, param);
        }
    }
}