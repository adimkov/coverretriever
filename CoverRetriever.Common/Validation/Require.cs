// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Require.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  The object invariant guard.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

using CoverRetriever.Common.Extensions;

namespace CoverRetriever.Common.Validation
{
    /// <summary>
    /// The object invariant guard.
    /// </summary>
    public static class Require
    {
        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> in case target is null.
        /// </summary>
        /// <param name="target">Target of validation.</param>
        /// <param name="paramName">Name of parameter.</param>
        public static void NotNull(object target, string paramName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(paramName, "{0} is null".FormatString(paramName));
            }
        }
    }
}
