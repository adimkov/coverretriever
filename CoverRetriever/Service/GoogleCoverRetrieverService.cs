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
    [Export(typeof(ICoverRetrieverService))]
    public class GoogleCoverRetrieverService : ICoverRetrieverService
    {
        /// <summary>
        /// Template to search covers.
        /// </summary>
        private const string BaseAddressParam = "?v=1.0&key={0}&rsz={1}&q={2}";

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
        /// Initializes a new instance of the <see cref="GoogleCoverRetrieverService"/> class.
        /// </summary>
        public GoogleCoverRetrieverService()
        {
            _baseAddress = Settings.Default.SearhGoogle + BaseAddressParam;
            _searchPattern = Settings.Default.SearhGooglePattern;
            _googleKey = Settings.Default.KeyGoogle;
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

            var requestedUri = _baseAddress.FormatString(_googleKey, coverCount, _searchPattern.FormatString(artist, album));

            return ObservableExtensions.Defer(
                observableJson.Finally(googleClient.Dispose).Select(
                            jsonResponce =>
                                {
                                    if (jsonResponce.EventArgs.Error != null)
                                    {
                                        throw new CoverSearchException("Unable to get response from google", jsonResponce.EventArgs.Error);
                                    }

                                    return this.ParseGoogleImageResponse(jsonResponce.EventArgs.Result).Take(coverCount);
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