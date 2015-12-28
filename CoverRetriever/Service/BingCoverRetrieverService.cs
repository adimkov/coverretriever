// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BingCoverRetrieverService.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2015. All rights reserved.  
// </copyright>
// <summary>
//  Service to grab covers through bing engine
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
    using Bing;

    using ObservableExtensions = System.Linq.ObservableExtensions;

    /// <summary>
    ///  Service to grab covers through google engine.
    /// </summary>
    //[Export(typeof(ICoverRetrieverService))]
    public class BingCoverRetrieverService : ICoverRetrieverService
    {



        private readonly BingSearchContainer _bingContainer;

        private readonly string _baseAddress;
        private readonly string _accountKey;
        private readonly string _searchPattern;
        /// Initializes a new instance of the <see cref="BingCoverRetrieverService"/> class.
        /// </summary>
        public BingCoverRetrieverService()
        {
            _baseAddress = Settings.Default.SearchBing;
            _bingContainer = new BingSearchContainer(new Uri(_baseAddress));

            // replace this value with your account key
            _accountKey = Settings.Default.KeyBing;
            _searchPattern = Settings.Default.SearhGooglePattern;
            // the next line configures the bingContainer to use your credentials.
            _bingContainer.Credentials = new NetworkCredential(_accountKey, _accountKey);

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

            var imageQuery = _bingContainer.Image(_searchPattern.FormatString(artist, album), null, null, null, null, null, null);

            var bingClient = new WebClient();
            bingClient.Credentials = new NetworkCredential(_accountKey, _accountKey);

            var observableJson = Observable.FromEventPattern<DownloadStringCompletedEventArgs>(bingClient, "DownloadStringCompleted");
            var uri = $"{imageQuery.RequestUri}&$format=JSON";

            return ObservableExtensions.Defer(
                observableJson.Finally(bingClient.Dispose).Select(
                            jsonResponce =>
                                {
                                    if (jsonResponce.EventArgs.Error != null)
                                    {
                                        throw new CoverSearchException("Unable to get response from google", jsonResponce.EventArgs.Error);
                                    }

                                    return ParseBingImageResponse(jsonResponce.EventArgs.Result).Take(coverCount);//jsonResponce.EventArgs.Result
                                }),
                () => bingClient.DownloadStringAsync(new Uri(uri))).Take(1);
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
                .Finally(downloader.Dispose)
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
        private IEnumerable<RemoteCover> ParseBingImageResponse(string jsonResponce)
        {
            dynamic covers = JsonConvert.DeserializeObject(jsonResponce);
            var result = new List<RemoteCover>();

            var entriesCount = covers.d.results.Count;
            for (int i = 0; i < entriesCount; i++)
            {
                var image = covers.d.results[i];
                var gImageUri = new Uri((string)image.MediaUrl, UriKind.Absolute);
                double width = (double)image.Width;
                double height = (double)image.Height;

                var tdGImage = new Uri((string)image.Thumbnail.MediaUrl, UriKind.Absolute);
                double tdwidth = (double)image.Thumbnail.Width;
                double tdheight = (double)image.Thumbnail.Height;
                result.Add(new RemoteCover(
                            image.ID.ToString(),
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