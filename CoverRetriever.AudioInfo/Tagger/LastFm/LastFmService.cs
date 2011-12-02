// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastFmService.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Base service of last.fm api
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo.Tagger.LastFm
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Last.fm api.
    /// </summary>
    public class LastFmService
    {
        /// <summary>
        /// The base address of last.fm api.
        /// </summary>
        private readonly string _serviceBaseAddress;

        /// <summary>
        /// The api key.
        /// </summary>
        private readonly string _apiKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastFmService"/> class. 
        /// </summary>
        /// <param name="serviceBaseAddress">
        /// The last fm service address.
        /// </param>
        /// <param name="apiKey">
        /// The API key.
        /// </param>
        public LastFmService(string serviceBaseAddress, string apiKey)
        {
            _serviceBaseAddress = serviceBaseAddress;
            _apiKey = apiKey;
        }

        /// <summary>
        /// Gets the track info.
        /// </summary>
        /// <param name="artist">The artist.</param>
        /// <param name="trackName">Name of the track.</param>
        /// <returns>Retrieved track info.</returns>
        public IObservable<XDocument> GetTrackInfo(string artist, string trackName)
        {
            var serviceAddress = new UriBuilder(_serviceBaseAddress);
            serviceAddress.Query = "method=track.getinfo&api_key={0}&artist={1}&track={2}".FormatString(_apiKey, artist, trackName);

            var webClient = new WebClient
                {
                    Encoding = Encoding.UTF8
                };
            var trackInfoObservable = Observable.FromEvent<DownloadStringCompletedEventArgs>(webClient, "DownloadStringCompleted");

            return trackInfoObservable.Defer(() => webClient.DownloadStringAsync(serviceAddress.Uri))
                .Select(
                    x =>
                    {
                        if (x.EventArgs.Error != null)
                        {
                            throw x.EventArgs.Error;
                        }

                        return XDocument.Parse(x.EventArgs.Result);
                    })
                .Take(1);
        }

        /// <summary>
        /// Gets the album info.
        /// </summary>
        /// <param name="artist">The artist.</param>
        /// <param name="albumName">Name of the album.</param>
        /// <returns>Retrieved album info.</returns>
        public IObservable<XDocument> GetAlbumInfo(string artist, string albumName)
        {
            var serviceAddress = new UriBuilder(_serviceBaseAddress);
            serviceAddress.Query = "method=album.getinfo&api_key={0}&artist={1}&album={2}".FormatString(_apiKey, artist, albumName);

            var webClient = new WebClient
                {
                    Encoding = Encoding.UTF8
                };

            var albumInfoObservable = Observable.FromEvent<DownloadStringCompletedEventArgs>(webClient, "DownloadStringCompleted");

            return albumInfoObservable.Defer(() => webClient.DownloadStringAsync(serviceAddress.Uri))
                .Select(
                    x =>
                    {
                        if (x.EventArgs.Error != null)
                        {
                            throw x.EventArgs.Error;
                        }

                        return XDocument.Parse(x.EventArgs.Result);
                    })
                .Take(1);
        }
    }
}