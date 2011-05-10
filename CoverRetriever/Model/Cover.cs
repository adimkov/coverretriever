using System;
using System.IO;
using System.Windows;

namespace CoverRetriever.Model
{
	public class Cover
	{
		public Cover()
		{
		}

		public Cover(Size coverSize, IObservable<Stream> stream)
		{
			CoverSize = coverSize;
			CoverStream = stream;
		}

		public Cover(Size coverSize, long length, IObservable<Stream> stream)
		{
			CoverSize = coverSize;
			Length = length;
			CoverStream = stream;
		}

		public Size CoverSize { get; protected set; }
		public long Length { get; protected set; }
		public IObservable<Stream> CoverStream { get; protected set; }
	}
}