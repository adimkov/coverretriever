using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;

using CoverRetriever.Model;
using CoverRetriever.Properties;
using Newtonsoft.Json;


//TODO: apply cache
namespace CoverRetriever.Service
{
	/// <summary>
	/// Google cover retriever
	/// </summary>
	[Export(typeof(ICoverRetrieverService))]
	public class GoogleCoverRetrieverService : ICoverRetrieverService
	{
		private const string BaseAddressParam = "?v=1.0&key={0}&rsz={1}&q={2}";
		private readonly string _baseAddress;
		private readonly string _searchPattern;

		private readonly string _googleKey;

		public GoogleCoverRetrieverService()
		{
			_baseAddress = Settings.Default.SearhGoogle + BaseAddressParam;
			_searchPattern = Settings.Default.SearhGooglePattern;
			_googleKey = Settings.Default.KeyGoogle;
		}

		/// <summary>
		/// Get cover for track
		/// </summary>
		/// <param name="artist">Artist name</param>
		/// <param name="album">Album</param>
		/// <param name="coverCount">count of cover. Range 1-8</param>
		/// <returns></returns>
		public IObservable<IEnumerable<RemoteCover>> GetCoverFor(string artist, string album, int coverCount)
		{
			if (coverCount < 0 || coverCount > 8)
			{
				throw new CoverSearchException("Invalid cover count size. Actual: {0}. Valid range: 1-8".FormatString(coverCount));
			}

			var googleClient = new WebClient();
			var observableJson = Observable.FromEvent<DownloadStringCompletedEventArgs>(googleClient, "DownloadStringCompleted");

			var requestedUri = _baseAddress.FormatString(_googleKey, coverCount, _searchPattern.FormatString(artist, album));
			googleClient.DownloadStringAsync(new Uri(requestedUri));
			return observableJson
				.Take(1)
				.Finally(googleClient.Dispose)
				.Select(
					jsonResponce =>
					{
						if (jsonResponce.EventArgs.Error != null)
						{
							throw new CoverSearchException("Unable to get response from google", jsonResponce.EventArgs.Error);
						}
						return ParseGoogleImageResponce(jsonResponce.EventArgs.Result).Take(coverCount);
					});
		}

		/// <summary>
		/// Download cover by uri
		/// </summary>
		/// <param name="coverUri">Uri of cover</param>
		/// <returns></returns>
		public IObservable<Stream> DownloadCover(Uri coverUri)
		{
			var downloader = new WebClient();
			var downloadOpservable = Observable.FromEvent<OpenReadCompletedEventArgs>(downloader, "OpenReadCompleted")
				.Select(
				x => 
				{
					if (x.EventArgs.Error != null)
					{
						throw new CoverSearchException(x.EventArgs.Error.Message, x.EventArgs.Error);
					}
					return x.EventArgs.Result;
				})
				.Take(1);

			var defferCaller = Observable.Defer(() =>
			{
				downloader.OpenReadAsync(coverUri);
				return downloadOpservable;
			});

			return defferCaller;
		}

		private IEnumerable<RemoteCover> ParseGoogleImageResponce(string jsonResponce)
		{
			dynamic covers = JsonConvert.DeserializeObject(jsonResponce);
			var result = new List<RemoteCover>();
			
			var entriesCount = covers.responseData.results.Count;
			for (int i = 0; i < entriesCount; i++)
			{
				var gimageSearch = covers.responseData.results[i];

				var gImageUri = new Uri((string)gimageSearch.url, UriKind.Absolute);
				double width = gimageSearch.width;
				double height = gimageSearch.height;

				var tdGImage = new Uri((string)gimageSearch.tbUrl, UriKind.Absolute);
				double tdwidth = gimageSearch.tbWidth;
				double tdheight = gimageSearch.tbHeight;
				result.Add(new RemoteCover(
				           	(string) gimageSearch.imageId,
							Path.GetFileName(gImageUri.AbsolutePath),
							new Size(width, height),
				           	new Size(tdwidth, tdheight),
				           	tdGImage,
							DownloadCover(gImageUri),
							DownloadCover(tdGImage)));
			}
			return result;
		}
	}
}