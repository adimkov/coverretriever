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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Provide tags from Last.fm site.
    /// </summary>
    public class LastFmTagger : ITagger
    {
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
        public LastFmTagger(string lastfmfpclientPath, string serviceBaseAddress, string apiKey)
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
                return _fingerprintResponse.IsSuccess && 
                       _trackInfoResponse.IsSuccess && 
                       _albumInfoResponse.IsSuccess;
            }
        }

        /// <summary>
        /// Loads the tags for audio file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Operation observable.</returns>
        public IObservable<Unit> LoadTagsForAudioFile(string fileName)
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
                            StandardOutputEncoding = Encoding.UTF8
                        };

                    var lastfmfpclientProcess = new Process
                        {
                            StartInfo = processStartInfo
                        };

                    lastfmfpclientProcess.Start();
                    
                    _fingerprintResponse.Parse(XDocument.Load(lastfmfpclientProcess.StandardOutput));

                    lastfmfpclientProcess.WaitForExit();
                    lastfmfpclientProcess.Close();
                }
                catch (Exception ex)
                {
                    throw new MetaProviderException("Unexpected Error obtaining tags from last.fm for file: " + fileName, ex);
                }
            }
            else
            {
                throw new MetaProviderException("lastfmfpclient.exe not found");
            }

            var trackInfoObserver = _lastFmService.GetTrackInfo(GetArtist(), GetTrackName())
                .Do(_trackInfoResponse.Parse)
                .Select(_ => new Unit());
            
            return trackInfoObserver;
        }

        /// <summary>
        /// Saves the tags in to specified File.
        /// </summary>
        /// <param name="tagRecipient">The tag recipient.</param>
        /// <returns>Operation observable.</returns>
        public IObservable<Unit> SaveTagsInTo(IMetaRecipient tagRecipient)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get an album name.
        /// </summary>
        /// <returns>The name of album.</returns>
        public string GetAlbum()
        {
            return _trackInfoResponse.SuggestedAlbums.FirstOrDefault();
        }

        /// <summary>
        /// Gets an artist.
        /// </summary>
        /// <returns>The artist.</returns>
        public string GetArtist()
        {
            return _fingerprintResponse.SuggestedArtists.FirstOrDefault();
        }

        /// <summary>
        /// Get year of album.
        /// </summary>
        /// <returns>The album year.</returns>
        public string GetYear()
        {
            return _albumInfoResponse.SuggestedYears.FirstOrDefault();
        }

        /// <summary>
        /// Gets name of track.
        /// </summary>
        /// <returns>The name of track.</returns>
        public string GetTrackName()
        {
            return _fingerprintResponse.SuggestedTrackNames.FirstOrDefault();
        }
    }
}