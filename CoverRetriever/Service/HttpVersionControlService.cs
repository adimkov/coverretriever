// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpVersionControlService.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Service to access the application version
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Service
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Net;
    using System.Reactive.Linq;
    using System.Xml.Linq;

    using CoverRetriever.Common.Infrastructure;
    using CoverRetriever.Model;

    /// <summary>
    /// Service to access the application version.
    /// </summary>
    [Export(typeof(IVersionControlService))]
    public class HttpVersionControlService : IVersionControlService
    {
        /// <summary>
        /// Web client for download xml.
        /// </summary>
        private readonly WebClient _xmlDownloader = new WebClient();

        /// <summary>
        /// Parser for xml described application versions.
        /// </summary>
        private readonly RevisionVersionParser _revisionVersionParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpVersionControlService"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="parser">The parser.</param>
        [ImportingConstructor]
        public HttpVersionControlService(
            [Import(ConfigurationKeys.VersionControlConnectionString)]string connectionString, 
            RevisionVersionParser parser)
        {
            ConnectionString = connectionString;
            _revisionVersionParser = parser;
        }

        /// <summary>
        /// Gets path to the file of latest version description.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Get latest version description.
        /// </summary>
        /// <returns>Observable of latest application version.</returns>
        public IObservable<RevisionVersion> GetLatestVersion()
        {
            var xmlDownloadedPush = Observable.FromEventPattern<OpenReadCompletedEventArgs>(_xmlDownloader, "OpenReadCompleted");
            
            _xmlDownloader.OpenReadAsync(new Uri(ConnectionString, UriKind.Absolute));

            return xmlDownloadedPush.Select(
                x =>
                {
                    if (x.EventArgs.Error != null)
                    {
                        throw x.EventArgs.Error;
                    }

                    return _revisionVersionParser.ParseVersionHistory(XDocument.Load(x.EventArgs.Result)).Max();
                })
                .Take(1);
        }
    }
}