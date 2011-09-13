// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteCover.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Cover from internet
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Model
{
    using System;
    using System.IO;
    using System.Windows;

    using CoverRetriever.AudioInfo;

    /// <summary>
    /// Cover from internet.
    /// </summary>
    public class RemoteCover : Cover, IComparable<RemoteCover>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteCover"/> class.
        /// </summary>
        public RemoteCover()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteCover"/> class.
        /// </summary>
        /// <param name="imageId">Id of the cover.</param>
        /// <param name="coverSize">Size of the cover.</param>
        public RemoteCover(string imageId, Size coverSize)
        {
            ImageId = imageId;
            CoverSize = coverSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteCover"/> class.
        /// </summary>
        /// <param name="imageId">Id of the cover.</param>
        /// <param name="name">The name of cover.</param>
        /// <param name="coverSize">Size of the cover.</param>
        /// <param name="thumbSize">Size of the cover thumb.</param>
        /// <param name="thumbUri">The cover thumb URI.</param>
        /// <param name="coverStream">The cover stream.</param>
        /// <param name="thumbStream">The cover thumb stream.</param>
        public RemoteCover(
            string imageId, 
            string name,
            Size coverSize, 
            Size thumbSize,
            Uri thumbUri,
            IObservable<Stream> coverStream,
            IObservable<Stream> thumbStream)
        {
            ImageId = imageId;
            Name = name;
            CoverSize = coverSize;
            CoverStream = coverStream;

            ThumbStream = thumbStream;
            ThumbSize = thumbSize;
            ThumbUri = thumbUri;
        }

        /// <summary>
        /// Gets the cover id.
        /// </summary>
        public string ImageId { get; private set; }

        /// <summary>
        /// Gets the size of the cover thumb.
        /// </summary>
        public Size ThumbSize { get; private set; }

        /// <summary>
        /// Gets or sets the cover thumb stream.
        /// </summary>
        /// <value>The thumb stream.</value>
        public IObservable<Stream> ThumbStream { get; set; }

        /// <summary>
        /// Gets the cover thumb URI.
        /// </summary>
        public Uri ThumbUri { get; private set; }

        #region Implementation of IComparable<in RemoteCover>

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(RemoteCover other)
        {
            if (CoverSize == other.CoverSize)
            {
                return 0;
            }

            return (CoverSize.Width * CoverSize.Height).CompareTo(other.CoverSize.Width * other.CoverSize.Height);
        }

        #endregion
    }
}