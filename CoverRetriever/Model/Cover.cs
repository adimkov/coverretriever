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

		public Cover(string name, Size coverSize, long length, IObservable<Stream> stream)
		{
			Name = name;
			CoverSize = coverSize;
			Length = length;
			CoverStream = stream;
		}

		public string Name { get; protected set; }
		public Size CoverSize { get; protected set; }
		public long Length { get; protected set; }
		public IObservable<Stream> CoverStream { get; protected set; }
	}
}