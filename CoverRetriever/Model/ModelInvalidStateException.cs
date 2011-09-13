// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelInvalidStateException.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Invalid state of object
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Model
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Invalid state of object.
    /// </summary>
    public class ModelInvalidStateException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelInvalidStateException"/> class.
        /// </summary>
        public ModelInvalidStateException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelInvalidStateException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="param">The param.</param>
        public ModelInvalidStateException(string message, params object[] param) 
            : base(string.Format(message, param))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelInvalidStateException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ModelInvalidStateException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelInvalidStateException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected ModelInvalidStateException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}