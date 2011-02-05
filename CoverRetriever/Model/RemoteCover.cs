using System;
using System.Windows;

namespace CoverRetriever.Model
{
	public class RemoteCover
	{
		public string ImageId { get; private set; }
		public Uri CoverUri { get; private set; }
		public Size CoverSize { get; private set; }
		public Uri ThumbCoverUri { get; private set; }
		public Size ThumbSize { get; private set; }

		public RemoteCover()
		{
		}

		public RemoteCover(string imageId, Uri coverUri, Size coverSize)
		{
			ImageId = imageId;
			CoverUri = coverUri;
			CoverSize = coverSize;
		}

		public RemoteCover(string imageId, Uri coverUri, Size coverSize, Uri thumbCoverUri, Size thumbSize)
		{
			ImageId = imageId;
			CoverUri = coverUri;
			CoverSize = coverSize;

			ThumbCoverUri = thumbCoverUri;
			ThumbSize = thumbSize;
		}
	}
}