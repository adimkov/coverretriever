using System;
using System.Collections.Generic;
using System.IO;

using CoverRetriever.Model;

namespace CoverRetriever.Service
{
	/// <summary>
	/// Cover retriever
	/// </summary>
	public interface ICoverRetrieverService
	{
		/// <summary>
		/// Get cover for track
		/// </summary>
		/// <param name="artist">Artist name</param>
		/// <param name="album">Album</param>
		/// <param name="coverCount">count of cover. Range 1-8</param>
		/// <returns></returns>
		IObservable<IEnumerable<RemoteCover>> GetCoverFor(string artist, string album, int coverCount);

		/// <summary>
		/// Download cover by uri
		/// </summary>
		/// <param name="coverUri">Uri of cover</param>
		/// <returns></returns>
		IObservable<Stream> DownloadCover(Uri coverUri);
	}
}