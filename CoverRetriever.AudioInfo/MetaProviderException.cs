using System;
using System.Runtime.Serialization;

namespace CoverRetriever.AudioInfo
{
	public class MetaProviderException : Exception
	{
		public MetaProviderException()
		{
		}

		public MetaProviderException(string message) : base(message)
		{
		}

		public MetaProviderException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected MetaProviderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}