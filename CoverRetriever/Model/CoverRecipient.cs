// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoverRecipient.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Determines recipient of cover.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Model
{
    /// <summary>
    /// Determines recipient of cover.
    /// </summary>
    public enum CoverRecipient
    {
        /// <summary>
        /// Directory is recipient.
        /// </summary>
        Directory = 0,

        /// <summary>
        /// Frame of file is recipient.
        /// </summary>
        Frame  = 1
    }
}