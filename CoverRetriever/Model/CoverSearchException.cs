using System;
using System.Runtime.Serialization;

namespace CoverRetriever.Model
{
	public class CoverSearchException : Exception
	{
		public CoverSearchException()
		{
		}

		public CoverSearchException(string message) : base(message)
		{
		}

		public CoverSearchException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected CoverSearchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}