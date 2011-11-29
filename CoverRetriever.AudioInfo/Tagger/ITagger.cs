﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITagger.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Provide tags for audio file
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo.Tagger
{
    using System;

    /// <summary>
    /// Provide tags for audio file.
    /// </summary>
    public interface ITagger : IMetaProvider
    {
        /// <summary>
        /// Loads the tags for audio file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Operation observable.</returns>
        IObservable<Unit> LoadTagsForAudioFile(string fileName);

        /// <summary>
        /// Saves the tags in to specified File.
        /// </summary>
        /// <param name="tagRecipient">The tag recipient.</param>
        /// <returns>Operation observable.</returns>
        IObservable<Unit> SaveTagsInTo(IMetaRecipient tagRecipient);
    }
}