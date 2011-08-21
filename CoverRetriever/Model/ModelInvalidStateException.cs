using System;
using System.Runtime.Serialization;

namespace CoverRetriever.Model
{
    /// <summary>
    /// Exception of invalid state of model
    /// </summary>
    public class ModelInvalidStateException : Exception
    {
        public ModelInvalidStateException()
        {
        }

        public ModelInvalidStateException(string message, params object[] param) 
            : base(String.Format(message, param))
        {
        }

        public ModelInvalidStateException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        protected ModelInvalidStateException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}