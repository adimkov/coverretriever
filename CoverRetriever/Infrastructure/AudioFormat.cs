// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioFormat.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Class provide supported audio files
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Infrastructure
{
    using System.Collections.Generic;

    /// <summary>
    /// Class provide supported audio files.
    /// </summary>
    public static class AudioFormat
    {
        /// <summary>
        /// Formats of supported audio formats.
        /// </summary>
        private static readonly IEnumerable<string> AudioFileExt = new[] { ".mp3", ".flac" };

        /// <summary>
        /// Gets supported audio files extensions.
        /// </summary>
        public static IEnumerable<string> AudioFileExtensions
        {
            get
            {
                return AudioFileExt;
            }
        }
    }
}