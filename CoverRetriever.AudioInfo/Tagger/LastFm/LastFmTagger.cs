// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastFmTagger.cs" author="Anton Dimkov">
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
    using System.Text;
    using System.Xml.Linq;

    using CoverRetriever.Common.Infrastructure;

    /// <summary>
    /// Provide tags from Last.fm site.
    /// </summary>
    [Export(typeof(ITagger))]
    public class LastFmTagger : ITagger
    {
        /// <summary>
        /// Safe file name in case to copy in temp folder.
        /// </summary>
        private const string SafeFileName = "0E42DCBE{0}";

        /// <summary>
        /// Path to 'lastfmfpclient' utility.
        /// </summary>
        private readonly string _lastfmfpclientPath;

        /// <summary>
        /// Fingerprint parser.
        /// </summary>
        private readonly FingerprintResponseParser _fingerprintResponse;

        /// <summary>
        /// The Last.fm API.
        /// </summary>
        private readonly LastFmService _lastFmService;

        /// <summary>
        /// Track info parser.
        /// </summary>
        private readonly TrackInfoResponseParser _trackInfoResponse;

        /// <summary>
        /// Album info parser.
        /// </summary>
        private readonly AlbumInfoResponseParser _albumInfoResponse;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastFmTagger"/> class.
        /// </summary>
        /// <param name="lastfmfpclientPath">The lastfmfpclient utility path.</param>
        /// <param name="serviceBaseAddress">The last fm service address.</param>
        /// <param name="apiKey">The API key.</param>
        [ImportingConstructor]
        public LastFmTagger(
            [Import(ConfigurationKeys.LastFmFingerprintClientPath)] string lastfmfpclientPath,
            [Import(ConfigurationKeys.LastFmServiceBaseAddress)] string serviceBaseAddress,
            [Import(ConfigurationKeys.LastFmApiKey)] string apiKey)
        {
            _lastfmfpclientPath = lastfmfpclientPath;
            _lastFmService = new LastFmService(serviceBaseAddress, apiKey);
            _fingerprintResponse = new FingerprintResponseParser();
            _trackInfoResponse = new TrackInfoResponseParser();
            _albumInfoResponse = new AlbumInfoResponseParser();
        }

        /// <summary>
        /// Gets a value indicating whether Meta Data empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return _fingerprintResponse.IsSuccess && _trackInfoResponse.IsSuccess && _albumInfoResponse.IsSuccess;
            }
        }

        /// <summary>
        /// Gets an album name.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors",
            Justification = "Reviewed. Suppression is OK here.")]
        public string Album
        {
            get
            {
                return _trackInfoResponse.SuggestedAlbums.FirstOrDefault();
            }

            set
            {
                throw new InvalidOperationException("Property does not support set value");
            }
        }

        /// <summary>
        /// Gets an artist.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors",
            Justification = "Reviewed. Suppression is OK here.")]
        public string Artist
        {
            get
            {
                return _fingerprintResponse.SuggestedArtists.FirstOrDefault();
            }

            set
            {
                throw new InvalidOperationException("Property does not support set value");
            }
        }

        /// <summary>
        /// Gets year of album.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors",
            Justification = "Reviewed. Suppression is OK here.")]
        public string Year
        {
            get
            {
                return _albumInfoResponse.SuggestedYears.FirstOrDefault();
            }

            set
            {
                throw new InvalidOperationException("Property does not support set value");
            }
        }

        /// <summary>
        /// Gets name of track.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors",
            Justification = "Reviewed. Suppression is OK here.")]
        public string TrackName
        {
            get
            {
                return _fingerprintResponse.SuggestedTrackNames.FirstOrDefault();
            }

            set
            {
                throw new InvalidOperationException("Property does not support set value");
            }
        }

        /// <summary>
        /// Loads the tags for audio file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Operation observable.</returns>
        public IObservable<Unit> LoadTagsForAudioFile(string fileName)
        {
            var fingerprindObserver = 
                Observable.Start(() => GetFingerprint(fileName))
                .Catch<Unit, IOException>(ex => Observable.Start(() => GetFingerprint(MakeSafeFileCopy(fileName))));

            var operationOpservable =
                fingerprindObserver.SelectMany(
                    x => _lastFmService.GetTrackInfo(Artist, TrackName).Do(_trackInfoResponse.Parse)).Select(
                        _ => new Unit());

            return operationOpservable;
        }

        /// <summary>
        /// Saves tags into file instance.
        /// </summary>
        public void Save()
        {
            throw new InvalidOperationException("Object does not support save operation");
        }

        /// <summary>
        /// Gets the fingerprint of audio file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void GetFingerprint(string fileName)
        {
            if (File.Exists(_lastfmfpclientPath))
            {
                try
                {
                    var processStartInfo = new ProcessStartInfo(_lastfmfpclientPath, " \"" + fileName + "\"")
                        {
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            ErrorDialog = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            StandardOutputEncoding = Encoding.UTF8
                        };

                    var lastfmfpclientProcess = new Process { StartInfo = processStartInfo };

                    lastfmfpclientProcess.Start();
                    lastfmfpclientProcess.WaitForExit();

                    if (lastfmfpclientProcess.ExitCode == 0)
                    {
                        _fingerprintResponse.Parse(XDocument.Load(lastfmfpclientProcess.StandardOutput));
                    }
                    else
                    {
                        var error =  new InvalidOperationException(
                            "Process finished with code: {0}\n\r{1}".FormatString(
                                lastfmfpclientProcess.ExitCode, lastfmfpclientProcess.StandardError.ReadToEnd()));
                        error.Data.Add("exitCode", lastfmfpclientProcess.ExitCode);

                        throw error;
                    }

                    lastfmfpclientProcess.Close();
                }
                catch (Exception ex)
                {
                    if (ex.Data.Contains("exitCode") && ex.Data["exitCode"].Equals(1))
                    {
                        throw new IOException("File does not found");
                    }

                    var errorMessage =
                        "Unexpected Error obtaining tags from last.fm for file: '{0}'\n\rDue: {1}".FormatString(
                            fileName, ex.Message);
                    throw new MetaProviderException(errorMessage, ex);
                }
            }
            else
            {
                throw new MetaProviderException("lastfmfpclient.exe not found");
            }
        }

        /// <summary>
        /// Makes the safe file copy.
        /// </summary>
        /// <param name="originalFile">The original file name.</param>
        /// <returns>Safe file path.</returns>
        private string MakeSafeFileCopy(string originalFile)
        {
            var extension = Path.GetExtension(originalFile);
            var newFilePath = Path.Combine(Path.GetTempPath(), SafeFileName.FormatString(extension));

            File.Copy(originalFile, newFilePath, true);
            Debug.WriteLine("The file '{0}' has unsafe symbols and it was copied to '{1}'".FormatString(originalFile, newFilePath));
            return newFilePath;
        }
    }
}