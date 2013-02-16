// -------------------------------------------------------------------------------------------------
// <copyright file="AcousticIdTaggerService.cs" author="Anton Dimkov">
//     Copyright (c) Anton Dimkov 2013. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------
namespace CoverRetriever.AudioInfo.Tagger.AcousticId
{
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Text;

    using CoverRetriever.Common.Infrastructure;

    /// <summary>
    /// Declaration of the <see cref="AcousticIdTaggerService" /> tagger service.
    /// </summary>
    [Export(typeof(ITaggerService))]
    public class AcousticIdTaggerService : ITaggerService
    {
        /// <summary>
        /// Safe file name in case to copy in temp folder.
        /// </summary>
        private const string SafeFileName = "0E42DCBE{0}";

        /// <summary>
        /// The acoustic id service.
        /// </summary>
        private readonly AcousticIdService acousticIdService;

        /// <summary>
        /// Path to 'fingerprint' utility.
        /// </summary>
        private readonly string fingerprintUtility;

        /// <summary>
        /// Initializes a new instance of the <see cref="AcousticIdTaggerService" /> class.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="fingerprintUtility">The fingerprint utility.</param>
        [ImportingConstructor]
        public AcousticIdTaggerService(
            [Import(ConfigurationKeys.AcousticIdApiKey)] string apiKey,
            [Import(ConfigurationKeys.AcousticIdFingerprintClientPath)] string fingerprintUtility)
        {
            acousticIdService = new AcousticIdService(apiKey);
            this.fingerprintUtility = fingerprintUtility;
        }

        /// <summary>
        /// Loads the tags for audio file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// Operation observable.
        /// </returns>
        public IObservable<IMetaProvider> LoadTagsForAudioFile(string fileName)
        {
            var fingerprintObserver = GetFingerprint(fileName)
                .Catch<Fingerprint, IOException>(ex => GetFingerprint(MakeSafeFileCopy(fileName)))
                .Trace("FingerprintClient");

            return fingerprintObserver
                .SelectMany(x => acousticIdService.Lookup(x))
                .Select(
                    x =>
                        {
                            var tag = new SuggestTag();
                            tag.Artist = AcousticResponseHelper.AggrigateArtist(x);
                            tag.TrackName = AcousticResponseHelper.AggrigateTrackName(x, tag.Artist);
                            tag.Album = AcousticResponseHelper.AggrigateAlbum(x, tag.Artist, tag.TrackName);
                            tag.Year = AcousticResponseHelper.AggrigateYear(x, tag.Artist, tag.Album, tag.TrackName).ToString();

                            return tag;
                        });
        }

        /// <summary>
        /// Gets the fingerprint of audio file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Response of fingerprint utility.</returns>
        /// <exception cref="System.IO.IOException">Error file access.</exception>
        /// <exception cref="MetaProviderException">Utility error.</exception>
        private IObservable<Fingerprint> GetFingerprint(string fileName)
        {
            return Observable.Start(
                () =>
                {
                    if (File.Exists(fingerprintUtility))
                    {
                        Process fingerprintProcess = null;
                        try
                        {
                            var processStartInfo = new ProcessStartInfo(
                                fingerprintUtility, " \"" + fileName + "\"")
                            {
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                ErrorDialog = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                StandardOutputEncoding = Encoding.UTF8,
                                //Arguments = "-length 40"
                            };

                            fingerprintProcess = new Process { StartInfo = processStartInfo };

                            fingerprintProcess.Start();
                            fingerprintProcess.WaitForExit();

                            if (fingerprintProcess.ExitCode == 0)
                            {
                                return FingerprintParser.Parse(fingerprintProcess.StandardOutput.ReadToEnd());
                            }

                            var error =
                                new InvalidOperationException(
                                    "Process finished with code: {0}\n\r{1}".FormatString(
                                        fingerprintProcess.ExitCode,
                                        fingerprintProcess.StandardError.ReadToEnd()));
                            error.Data.Add("exitCode", fingerprintProcess.ExitCode);

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
                            if (fingerprintProcess != null)
                            {
                                fingerprintProcess.Close();
                            }
                        }
                    }

                    throw new MetaProviderException("{0} not found".FormatString(fingerprintUtility));
                });
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
            Trace.TraceWarning("The file '{0}' has unsafe symbols and it was copied to '{1}'".FormatString(originalFile, newFilePath));
            return newFilePath;
        }
    }
}