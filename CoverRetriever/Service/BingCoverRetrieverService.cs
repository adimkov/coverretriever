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
    using System.Text.RegularExpressions;

    using CoverRetriever.Model;
    using CoverRetriever.Properties;
    using Bing;

    using ObservableExtensions = System.Linq.ObservableExtensions;
    using CsQuery;

    /// <summary>
    ///  Service to grab covers through google engine.
    /// </summary>
    [Export(typeof(ICoverRetrieverService))]
    public class BingCoverRetrieverService : ICoverRetrieverService
    {


        private readonly string _baseAddress;
        private readonly string _searchPattern;
        /// Initializes a new instance of the <see cref="BingCoverRetrieverService"/> class.
        /// </summary>
        public BingCoverRetrieverService()
        {
            _baseAddress = Settings.Default.SearchBing;

            _searchPattern = Settings.Default.SearhGooglePattern;

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

            var query = _searchPattern.FormatString(artist, album);

            var bingClient = new WebClient();

            var observableJson = Observable.FromEventPattern<DownloadStringCompletedEventArgs>(bingClient, "DownloadStringCompleted");
            var uri = _baseAddress.FormatString(query);

            return ObservableExtensions.Defer(
                observableJson.Finally(bingClient.Dispose).Select(
                            jsonResponce =>
                                {
                                    if (jsonResponce.EventArgs.Error != null)
                                    {
                                        throw new CoverSearchException("Unable to get response from bing", jsonResponce.EventArgs.Error);
                                    }

                                    return ParseBingImageResponse(query, coverCount);
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
        /// Download cover by uri.
        /// </summary>
        /// <param name="coverUri">Uri of cover.</param>
        /// <returns>Sream of cover.</returns>
        public IObservable<Stream> DownloadCover(Uri coverUri, Uri thumbUri)
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

                                return DownloadCover(thumbUri).SingleOrDefault();
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
        private IEnumerable<RemoteCover> ParseBingImageResponse(string query, int coverCount)
        {

            var result = new List<RemoteCover>();

            CQ dom = CQ.CreateFromUrl($"https://www.bing.com/images/search?q={query}");
            var items = dom[".item"].Take(coverCount);
            foreach (var item in items)
            {
                var image = dom[item].Find(".thumb");
                var imgInfo = dom[item].Find(".fileInfo");
                var imageThumb = dom[item].Find("img");

                Regex r = new Regex(@"(?<width>\d+) x (?<height>\d+).", RegexOptions.None);
                Match m = r.Match(imgInfo.Text());


                var bImageUri = new Uri(image.Attr("href"), UriKind.Absolute);
                double width = 0;
                double height = 0;
                if (m.Success)
                {
                    width = int.Parse(r.Match(imgInfo.Text()).Result("${width}"));
                    height = int.Parse(r.Match(imgInfo.Text()).Result("${height}"));
                }

                var tdGImage = new Uri(imageThumb.Attr("src"), UriKind.Absolute);
                double tdwidth = double.Parse(imageThumb.Attr("width"));
                double tdheight = double.Parse(imageThumb.Attr("height"));

                result.Add(new RemoteCover(
                            image.Attr("h"),
                            Path.GetFileName(bImageUri.AbsolutePath),
                            new Size(width, height),
                            new Size(tdwidth, tdheight),
                            tdGImage,
                            DownloadCover(bImageUri, tdGImage),
                            DownloadCover(tdGImage)));
            }


            return result;
        }
    }
}