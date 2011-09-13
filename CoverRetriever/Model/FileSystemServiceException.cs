// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemServiceException.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Exception of operations with file system
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Model
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception of operations with file system. 
    /// </summary>
    public class FileSystemServiceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemServiceException"/> class.
        /// </summary>
        public FileSystemServiceException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemServiceException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public FileSystemServiceException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemServiceException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public FileSystemServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemServiceException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected FileSystemServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}