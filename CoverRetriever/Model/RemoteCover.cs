using System;
using System.IO;
using System.Windows;

namespace CoverRetriever.Model
{
	public class RemoteCover : Cover, IComparable<RemoteCover>
	{
		public string ImageId { get; private set; }
		public Size ThumbSize { get; private set; }
		public IObservable<Stream> ThumbStream { get; set; }
		public Uri ThumbUri { get; private set; }

		public RemoteCover()
		{
		}

		public RemoteCover(string imageId, Size coverSize)
		{
			ImageId = imageId;
			CoverSize = coverSize;
		}

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