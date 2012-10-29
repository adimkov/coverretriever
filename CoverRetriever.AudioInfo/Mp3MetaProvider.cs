// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mp3MetaProvider.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Tag provider of mp3 audio
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;

    using TagLib;

    using File = TagLib.File;

    /// <summary>
    /// Tag provider of mp3 audio.
    /// </summary>
    [Export("mp3", typeof(IMetaProvider))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Mp3MetaProvider : AudioFileMetaProvider
    {
        /// <summary>
        /// Initializes static members of the <see cref="Mp3MetaProvider"/> class. 
        /// </summary>
        static Mp3MetaProvider()
        {
            TagLib.Id3v1.Tag.DefaultStringHandler = new AutoStringHandler();
            TagLib.Id3v2.Tag.DefaultEncoding = StringType.Latin1;
            ByteVector.UseBrokenLatin1Behavior = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mp3MetaProvider"/> class.
        /// </summary>
        [Obsolete("Added for MEF compatibility")]
        public Mp3MetaProvider()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mp3MetaProvider"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public Mp3MetaProvider(string fileName)
        {
            Activate(fileName);
        }
    }
}