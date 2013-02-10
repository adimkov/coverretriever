// -------------------------------------------------------------------------------------------------
// <copyright file="AcousticIdTagger.cs" author="Anton Dimkov">
//     Copyright (c) Anton Dimkov 2013. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------
namespace CoverRetriever.AudioInfo.Tagger
{
    using System;

    public class AcousticIdTagger : ITaggerService
    {
        public IObservable<IMetaProvider> LoadTagsForAudioFile(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}