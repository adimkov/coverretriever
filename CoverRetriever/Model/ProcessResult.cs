// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessResult.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  State of the operation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Model
{
    /// <summary>
    /// State of the operation.
    /// </summary>
    public enum ProcessResult
    {
        /// <summary>
        /// Operation is started.
        /// </summary>
        Begin = 1,

        /// <summary>
        /// Operation is completed.
        /// </summary>
        Done = 2,

        /// <summary>
        /// Operation was failed
        /// </summary>
        Error = 3
    }
}