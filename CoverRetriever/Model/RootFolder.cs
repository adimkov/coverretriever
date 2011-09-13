// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootFolder.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  The root folder to analyze
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Model
{
    /// <summary>
    /// The root folder to analyze.
    /// </summary>
    public class RootFolder : Folder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootFolder"/> class.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        public RootFolder(string rootPath)
            : base(rootPath)
        {
        }
    }
}