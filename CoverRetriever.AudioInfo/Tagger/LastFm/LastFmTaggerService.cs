// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastFmTaggerService.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Provide tags from Last.fm site.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo.Tagger.LastFm
{
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Text;
    using System.Xml.Linq;

    using CoverRetriever.Common.Infrastructure;

    /// <summary>
    /// Provide tags from Last.fm site.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    //[Export(typeof(ITaggerService))]
    public class LastFmTaggerService : ITaggerService
    {
        /// <summary>
        /// Safe file name in case to copy in temp folder.
        /// </summary>
        private const string SafeFileName = "0E42DCBE{0}";

        /// <summary>
        /// Path to 'lastfmfpclient' utility.
        /// </summary>
        private readonly string lastfmfpclientPath;

        /// <summary>
        /// The Last.fm API.
        /// </summary>
        private readonly LastFmService lastFmService;

        /// <summary>
        /// Fingerprint parser.
        /// </summary>
        private readonly FingerprintResponseParser fingerprintResponse;

        /// <summary>
        /// Track info parser.
        /// </summary>
        private readonly TrackInfoResponseParser trackInfoResponse;

        /// <summary>
        /// Album info parser.
        /// </summary>
        private readonly AlbumInfoResponseParser albumInfoResponse;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastFmTaggerService"/> class.
        /// </summary>
        /// <param name="lastfmfpclientPath">The lastfmfpclient utility path.</param>
        /// <param name="serviceBaseAddress">The last fm service address.</param>
        /// <param name="apiKey">The API key.</param>
        [ImportingConstructor]
        public LastFmTaggerService(
            [Import(ConfigurationKeys.LastFmFingerprintClientPath)] string lastfmfpclientPath,
            [Import(ConfigurationKeys.LastFmServiceBaseAddress)] string serviceBaseAddress,
            [Import(ConfigurationKeys.LastFmApiKey)] string apiKey)
        {
            this.lastfmfpclientPath = lastfmfpclientPath;
            this.lastFmService = new LastFmService(serviceBaseAddress, apiKey);
            this.fingerprintResponse = new FingerprintResponseParser();
            this.trackInfoResponse = new TrackInfoResponseParser();
            this.albumInfoResponse = new AlbumInfoResponseParser();
        }

        /// <summary>
        /// Loads the tags for audio file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Operation observable.</returns>
        public IObservable<IMetaProvider> LoadTagsForAudioFile(string fileName)
        {
            CleanOffParsers();
            var suggestedTag = new SuggestTag();
            var fingerprintObserver = GetFingerprint(fileName)
                .Catch<XDocument, IOException>(ex => GetFingerprint(MakeSafeFileCopy(fileName)))
                .Trace("FingerprintClient");

            var operationOpservable = fingerprintObserver
                .Do(fi =>
                {
                    fingerprintResponse.Parse(fi);
                    suggestedTag.Artist = this.fingerprintResponse.SuggestedArtists.FirstOrDefault();
                    suggestedTag.TrackName = this.fingerprintResponse.SuggestedTrackNames.FirstOrDefault();
                })
                .SelectMany(x => this.lastFmService.GetTrackInfo(suggestedTag.Artist, suggestedTag.TrackName)
                    .Do(ti =>
                    {
                        this.trackInfoResponse.Parse(ti);
                        suggestedTag.Album = this.trackInfoResponse.SuggestedAlbums.FirstOrDefault();
                    })
                    .Trace("TrackInfo"))
                 .SelectMany(x => this.lastFmService.GetAlbumInfo(suggestedTag.Artist, suggestedTag.Album)
                    .Do(
                    ai =>
                    {
                        this.albumInfoResponse.Parse(ai);
                        suggestedTag.Year = this.albumInfoResponse.SuggestedYears.FirstOrDefault();
                    })
                    .Trace("AlbumInfo"))
                .Select(_ => suggestedTag);

            return operationOpservable;
        }

        /// <summary>
        /// Gets the fingerprint of audio file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Response of fingerprint utility.</returns>
        /// <exception cref="System.IO.IOException">Error file access.</exception>
        /// <exception cref="MetaProviderException">Utility error.</exception>
        private IObservable<XDocument> GetFingerprint(string fileName)
        {
            return Observable.Start(
                () =>
                    {
                        if (File.Exists(this.lastfmfpclientPath))
                        {
                            Process lastfmfpclientProcess = null;
                            try
                            {
                                var processStartInfo = new ProcessStartInfo(
                                    this.lastfmfpclientPath, " \"" + fileName + "\"")
                                    {
                                        UseShellExecute = false,
                                        CreateNoWindow = true,
                                        ErrorDialog = false,
                                        RedirectStandardOutput = true,
                                        RedirectStandardError = true,
                                        StandardOutputEncoding = Encoding.UTF8
                                    };

                                lastfmfpclientProcess = new Process { StartInfo = processStartInfo };

                                lastfmfpclientProcess.Start();
                                lastfmfpclientProcess.WaitForExit();

                                if (lastfmfpclientProcess.ExitCode == 0)
                                {
                                    return XDocument.Load(lastfmfpclientProcess.StandardOutput);
                                }

                                var error =
                                    new InvalidOperationException(
                                        "Process finished with code: {0}\n\r{1}".FormatString(
                                            lastfmfpclientProcess.ExitCode,
                                            lastfmfpclientProcess.StandardError.ReadToEnd()));
                                error.Data.Add("exitCode", lastfmfpclientProcess.ExitCode);

                                throw error;
                            }
                            catch (Exception ex)
                            {
                                if (ex.Data.Contains("exitCode") && ex.Data["exitCode"].Equals(1))
                                {
                                    throw new IOException("File does not found");
                                }

                                var errorMessage =
                                    "Unexpected Error obtaining tags from last.fm for file: '{0}'\n\rDue: {1}".
                                        FormatString(fileName, ex.Message);
                                throw new MetaProviderException(errorMessage, ex);
                            }
                            finally
                            {
                                if (lastfmfpclientProcess != null)
                                {
                                    lastfmfpclientProcess.Close();
                                }
                            }
                        }

                        throw new MetaProviderException("lastfmfpclient.exe not found");
                    });
        }

        /// <summary>
        /// Makes the safe file copy.
        /// </summary>
        /// <param name="originalFile">The original file name.</param>
        /// <returns>Safe file path.</returns>
        private string MakeSafeFileCopy(string originalFile)
        {
            CleanOffParsers();
            var extension = Path.GetExtension(originalFile);
            var newFilePath = Path.Combine(Path.GetTempPath(), SafeFileName.FormatString(extension));

            File.Copy(originalFile, newFilePath, true);
            Trace.TraceWarning("The file '{0}' has unsafe symbols and it was copied to '{1}'".FormatString(originalFile, newFilePath));
            return newFilePath;
        }

        /// <summary>
        /// Cleans off the parsers.
        /// </summary>
        private void CleanOffParsers()
        {
            this.fingerprintResponse.Clear();
            this.trackInfoResponse.Clear();
            this.albumInfoResponse.Clear();
        }
    }
}