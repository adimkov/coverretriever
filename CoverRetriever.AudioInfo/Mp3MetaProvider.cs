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

        /// <summary>
        /// Indicate is Meta Data empty.
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                EnsureInstanceWasNotDisposed();
                return File.GetTag(TagTypes.Id3v2).IsEmpty &&
                    File.GetTag(TagTypes.Id3v1).IsEmpty;
            }
        }

        /// <summary>
        /// Gets an album name.
        /// </summary>
        /// <returns>
        /// Name of album.
        /// </returns>
        public override string GetAlbum()
        {
            EnsureInstanceWasNotDisposed();
            var id3 = GetAudioTag(File, x => !string.IsNullOrEmpty(x.Album));
            return id3.Album ?? base.GetAlbum();
        }

        /// <summary>
        /// Gets an artist.
        /// </summary>
        /// <returns>
        /// Name of artist.
        /// </returns>
        public override string GetArtist()
        {
            EnsureInstanceWasNotDisposed();
            var id3 = GetAudioTag(File, x => !string.IsNullOrEmpty(x.FirstPerformer));
            return id3.FirstPerformer ?? base.GetArtist();
        }

        /// <summary>
        /// Get name  of track.
        /// </summary>
        /// <returns>
        /// Name of track.
        /// </returns>
        public override string GetTrackName()
        {
            EnsureInstanceWasNotDisposed();
            var id3 = GetAudioTag(File, x => !string.IsNullOrEmpty(x.Title));
            return id3.Title ?? base.GetTrackName();
        }

        /// <summary>
        /// Get year of album.
        /// </summary>
        /// <returns>
        /// Year of composition.
        /// </returns>
        public override string GetYear()
        {
            EnsureInstanceWasNotDisposed();
            var id3 = GetAudioTag(File, x => x.Year > 0);
            return id3.Year > 0 ? id3.Year.ToString() : base.GetYear();
        }

        /// <summary>
        /// Get tab from file.
        /// <remarks>
        /// IDv2 has highest priority.
        /// </remarks>
        /// </summary>
        /// <param name="file">Audio file.</param>
        /// <param name="isCorrect">Validator for ñheck tag correct. If tag is invalid, try to get another tag.</param>
        /// <returns>
        /// The Tag.
        /// </returns>
        private Tag GetAudioTag(File file, Func<Tag, bool> isCorrect)
        {
            if ((file.TagTypesOnDisk & TagTypes.Id3v2) == TagTypes.Id3v2 && 
                isCorrect(file.GetTag(TagTypes.Id3v2)))
            {
                return file.GetTag(TagTypes.Id3v2);
            }

            return file.GetTag(TagTypes.Id3v1);
        }
    }
}