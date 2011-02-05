using System;
using System.Runtime.Serialization;

namespace CoverRetriever.Model
{
	public class FileSystemServiceException : Exception
	{
		public FileSystemServiceException()
		{
		}

		public FileSystemServiceException(string message) 
			: base(message)
		{
		}

		public FileSystemServiceException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected FileSystemServiceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}