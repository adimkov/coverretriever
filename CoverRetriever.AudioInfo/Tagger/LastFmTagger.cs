// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastFmTagger.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Provide tags from Last.fm site.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo.Tagger
{
    using System;
    using System.Diagnostics;
    using System.IO;
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
        /// Initializes a new instance of the <see cref="LastFmTagger"/> class.
        /// </summary>
        /// <param name="lastfmfpclientPath">The lastfmfpclient utility path.</param>
        public LastFmTagger(string lastfmfpclientPath)
        {
            _lastfmfpclientPath = lastfmfpclientPath;
        }

        /// <summary>
        /// Gets the tags for audio file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void GetTagsForAudioFile(string fileName)
        {
            Process lastfmfpclientProcess;
            ProcessStartInfo processStartInfo;

            if (File.Exists(_lastfmfpclientPath))
            {
                try
                {
                    processStartInfo = new ProcessStartInfo(_lastfmfpclientPath, " \"" + fileName + "\"");
                    processStartInfo.UseShellExecute = false;
                    processStartInfo.CreateNoWindow = true;
                    processStartInfo.ErrorDialog = false;
                    processStartInfo.RedirectStandardOutput = true;
                    processStartInfo.StandardOutputEncoding = Encoding.UTF8;
                    
                    lastfmfpclientProcess = new Process();
                    lastfmfpclientProcess.StartInfo = processStartInfo;
                    lastfmfpclientProcess.Start();
                    
                    var document = XDocument.Load(lastfmfpclientProcess.StandardOutput);

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
        }

        /// <summary>
        /// Saves the tags in to specified File.
        /// </summary>
        /// <param name="tagRecipient">The tag recipient.</param>
        public void SaveTagsInTo(IMetaRecipient tagRecipient)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Indicate is Meta Data empty
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Get an album name
        /// </summary>
        /// <returns></returns>
        public string GetAlbum()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get an artist
        /// </summary>
        /// <returns></returns>
        public string GetArtist()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get year of album
        /// </summary>
        /// <returns></returns>
        public string GetYear()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get name  of track
        /// </summary>
        /// <returns></returns>
        public string GetTrackName()
        {
            throw new System.NotImplementedException();
        }
    }
}