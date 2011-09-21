// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlacMetaProvider.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Tag provider of flac audio
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo
{
    using System;
    using System.ComponentModel.Composition;

    /// <summary>
    /// Tag provider of flac audio.
    /// </summary>
    [Export("flac", typeof(IMetaProvider))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FlacMetaProvider : AudioFileMetaProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlacMetaProvider"/> class.
        /// </summary>
        [Obsolete("Added for MEF compatibility")]
        public FlacMetaProvider()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlacMetaProvider"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public FlacMetaProvider(string filePath)
        {
            Activate(filePath);
        }
    }
}