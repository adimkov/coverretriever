// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITagger.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Provide tags for audio file
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo.Tagger
{
    /// <summary>
    /// Provide tags for audio file.
    /// </summary>
    public interface ITagger : IMetaProvider
    {
        /// <summary>
        /// Gets the tags for audio file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        void GetTagsForAudioFile(string fileName);

        /// <summary>
        /// Saves the tags in to specified File.
        /// </summary>
        /// <param name="tagRecipient">The tag recipient.</param>
        void SaveTagsInTo(IMetaRecipient tagRecipient);
    }
}