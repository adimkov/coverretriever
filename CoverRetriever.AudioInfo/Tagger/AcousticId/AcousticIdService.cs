// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AcousticIdService.cs" author="Anton Dimkov">
//     Copyright (c) Anton Dimkov 2013. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.AudioInfo.Tagger.AcousticId
{
    using System;
    using System.Net;
    using System.Reactive.Linq;

    using Newtonsoft.Json;

    /// <summary>
    /// Declaration of the <see cref="AcousticIdService" /> service.
    /// </summary>
    internal class AcousticIdService
    {
        /// <summary>
        /// The service key.
        /// </summary>
        private readonly string serviceKey;

        /// <summary>
        /// The request template.
        /// </summary>
        private string requestTemplate = "http://api.acoustid.org/v2/lookup?client={0}&&meta=releases+tracks&duration={1}&fingerprint={2}";

        /// <summary>
        /// Initializes a new instance of the <see cref="AcousticIdService" /> class.
        /// </summary>
        /// <param name="serviceKey">The service key.</param>
        public AcousticIdService(string serviceKey)
        {
            this.serviceKey = serviceKey;
        }

        /// <summary>
        /// Lookups the specified duration.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="fingerprint">The fingerprint.</param>
        /// <returns>Acoustic response.</returns>
        public IObservable<AcousticResponse> Lookup(int duration, string fingerprint)
        {
            var accousticIdServiceClient = new WebClient();
            var observableJson = Observable.FromEventPattern<DownloadStringCompletedEventArgs>(accousticIdServiceClient, "DownloadStringCompleted");
            var result = observableJson.Select(
                x =>
                    {
                        if (x.EventArgs.Error == null)
                        {
                            return JsonConvert.DeserializeObject<AcousticResponse>(x.EventArgs.Result);
                        }

                        throw x.EventArgs.Error;
                    })
                    .Take(1);

            accousticIdServiceClient.DownloadStringAsync(new Uri(requestTemplate.FormatString(serviceKey, duration, fingerprint)));
            return result;
        }      
    }
}