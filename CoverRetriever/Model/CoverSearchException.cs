// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoverSearchException.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Exception of searching cover.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Model
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception of searching cover.
    /// </summary>
    public class CoverSearchException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoverSearchException"/> class.
        /// </summary>
        public CoverSearchException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverSearchException"/> class.
        /// </summary>
        /// <param name="message">The message of exception.</param>
        public CoverSearchException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverSearchException"/> class.
        /// </summary>
        /// <param name="message">The message of exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public CoverSearchException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverSearchException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null. 
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). 
        /// </exception>
        protected CoverSearchException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}