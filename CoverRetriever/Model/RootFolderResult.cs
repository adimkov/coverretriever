// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootFolderResult.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Result of choose the folder to analyze
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Model
{
    /// <summary>
    /// Result of root folder selecting.
    /// </summary>
    public struct RootFolderResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootFolderResult"/> struct.
        /// </summary>
        /// <param name="rootFolder">The root folder.</param>
        public RootFolderResult(string rootFolder) 
            : this()
        {
            HasValue = true;
            RootFolder = rootFolder;
        }

        /// <summary>
        /// Gets a value indicating whether does user chose folder.
        /// </summary>
        public bool HasValue { get; private set; }

        /// <summary>
        /// Gets selected folder.
        /// </summary>
        public string RootFolder { get; private set; }
    }
}