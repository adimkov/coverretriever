// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoogleCoverRetrieverService.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Service to grab covers through google engine
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Service
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reactive.Linq;
    using System.Windows;

    using CoverRetriever.Model;
    using CoverRetriever.Properties;

    using Newtonsoft.Json;

    using ObservableExtensions = System.Linq.ObservableExtensions;

    /// <summary>
    ///  Service to grab covers through google engine.
    /// </summary>
    [Export("GoogleService", typeof(ICoverRetrieverService))]
    public class GoogleCoverRetrieverService : ICoverRetrieverService
    {
        /// <summary>
        /// Template to search covers.
        /// </summary>
        private const string BaseAddressParam = "?key={0}&cx={1}&num={2}&q={3}&searchType=image";

        /// <summary>
        /// Google search address.
        /// </summary>
        private readonly string _baseAddress;

        /// <summary>
        /// Search pattern of covers.
        /// </summary>
        private readonly string _searchPattern;

        /// <summary>
        /// Google developer key.
        /// </summary>
        private readonly string _googleKey;

        /// <summary>
        /// Google developer key.
        /// </summary>
        private readonly string _googleCX;
        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleCoverRetrieverService"/> class.
        /// </summary>
        public GoogleCoverRetrieverService()
        {
            _baseAddress = Settings.Default.SearhGoogle + BaseAddressParam;
            _searchPattern = Settings.Default.SearhGooglePattern;
            _googleKey = Settings.Default.KeyGoogle;
            _googleCX = Settings.Default.SearchGoogleCX;
        }

        /// <summary>
        /// Get cover for audio track.
        /// </summary>
        /// <param name="artist">Artist name.</param>
        /// <param name="album">Album name.</param>
        /// <param name="coverCount">Count of cover. Range 1-8.</param>
        /// <returns>Found covers.</returns>
        public IObservable<IEnumerable<RemoteCover>> GetCoverFor(string artist, string album, int coverCount)
        {
            if (coverCount < 0 || coverCount > 8)
            {
                throw new CoverSearchException("Invalid cover count size. Actual: {0}. Valid range: 1-8".FormatString(coverCount));
            }

            var googleClient = new WebClient();
            var observableJson = Observable.FromEventPattern<DownloadStringCompletedEventArgs>(googleClient, "DownloadStringCompleted");

            var requestedUri = _baseAddress.FormatString(_googleKey, _googleCX, coverCount, _searchPattern.FormatString(artist, album));

            return ObservableExtensions.Defer(
                observableJson.Finally(googleClient.Dispose).Select(
                            jsonResponce =>
                                {
                                    if (jsonResponce.EventArgs.Error != null)
                                    {
                                        throw new CoverSearchException("Unable to get response from google", jsonResponce.EventArgs.Error);
                                    }

                                    return ParseGoogleImageResponse(jsonResponce.EventArgs.Result).Take(coverCount);
                                }),
                () => googleClient.DownloadStringAsync(new Uri(requestedUri))).Take(1);
        }

        /// <summary>
        /// Download cover by uri.
        /// </summary>
        /// <param name="coverUri">Uri of cover.</param>
        /// <returns>Sream of cover.</returns>
        public IObservable<Stream> DownloadCover(Uri coverUri)
        {
            ////todo: implement a caching
            var downloader = new WebClient();
            var downloadOpservable = ObservableExtensions.Defer(Observable.FromEventPattern<DownloadDataCompletedEventArgs>(downloader, "DownloadDataCompleted")
                    .Select(
                        x =>
                            {
                                if (x.EventArgs.Error != null)
                                {
                                    throw new CoverSearchException(x.EventArgs.Error.Message, x.EventArgs.Error);
                                }

                                return new MemoryStream(x.EventArgs.Result);
                            }), () => downloader.DownloadDataAsync(coverUri))
                .Take(1);

            return downloadOpservable;
        }

        /// <summary>
        /// Parses the google image response.
        /// </summary>
        /// <param name="jsonResponce">The json response.</param>
        /// <returns>The list of covers.</returns>
        private IEnumerable<RemoteCover> ParseGoogleImageResponse(string jsonResponce)
        {
            dynamic covers = JsonConvert.DeserializeObject(jsonResponce);
            var result = new List<RemoteCover>();

            var entriesCount = covers.items.Count;
            for (int i = 0; i < entriesCount; i++)
            {
                var gimageSearch = covers.items[i];

                var gImageUri = new Uri((string)gimageSearch.link, UriKind.Absolute);
                double width = gimageSearch.image.width;
                double height = gimageSearch.image.height;

                var tdGImage = new Uri((string)gimageSearch.image.thumbnailLink, UriKind.Absolute);
                double tdwidth = gimageSearch.image.thumbnailWidth;
                double tdheight = gimageSearch.image.thumbnailHeight;
                result.Add(new RemoteCover(
                            (string)gimageSearch.title,
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