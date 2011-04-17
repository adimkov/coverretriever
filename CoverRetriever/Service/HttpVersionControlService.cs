using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Xml.Linq;

using CoverRetriever.Infrastructure;
using CoverRetriever.Model;

namespace CoverRetriever.Service
{
	[Export(typeof(IVersionControlService))]
	public class HttpVersionControlService : IVersionControlService
	{
		private readonly WebClient _xmlDownloader = new WebClient();
		private readonly RevisionVersionParser _revisionVersionParser;

		[ImportingConstructor]
		public HttpVersionControlService(
			[Import(ConfigurationKeys.VersionControlConnectionString)]string connectionString, 
			RevisionVersionParser parser)
		{
			ConnectionString = connectionString;
			_revisionVersionParser = parser;
		}

		/// <summary>
		/// Path to the file of latest version description
		/// </summary>
		public string ConnectionString { get; private set; }

		/// <summary>
		/// Get latest version description
		/// </summary>
		/// <returns></returns>
		public IObservable<RevisionVersion> GetLatestVersion()
		{
			var xmlDownloadedPush = Observable.FromEvent<OpenReadCompletedEventArgs>(_xmlDownloader, "OpenReadCompleted");
			
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