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
        public AcousticIdTaggerService(
            [Import(ConfigurationKeys.AccousticIdApiKey)] string apiKey,
            [Import(ConfigurationKeys.AccousticIdFingerprintClientPath)] string fingerprintUtility)
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
            var suggestedTag = new SuggestTag();
            var fingerprintObserver = GetFingerprint(fileName)
                .Catch<string, IOException>(ex => GetFingerprint(MakeSafeFileCopy(fileName)))
                .Trace("FingerprintClient");

            //fingerprintUtility.SelectMany()

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the fingerprint of audio file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Response of fingerprint utility.</returns>
        /// <exception cref="System.IO.IOException">Error file access.</exception>
        /// <exception cref="MetaProviderException">Utility error.</exception>
        private IObservable<string> GetFingerprint(string fileName)
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
                                StandardOutputEncoding = Encoding.UTF8
                            };

                            fingerprintProcess = new Process { StartInfo = processStartInfo };

                            fingerprintProcess.Start();
                            fingerprintProcess.WaitForExit();

                            if (fingerprintProcess.ExitCode == 0)
                            {
                                return fingerprintProcess.StandardOutput.ReadToEnd();
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
            var extension = Path.GetExtension(originalFile);
            var newFilePath = Path.Combine(Path.GetTempPath(), SafeFileName.FormatString(extension));

            File.Copy(originalFile, newFilePath, true);
            Trace.TraceWarning("The file '{0}' has unsafe symbols and it was copied to '{1}'".FormatString(originalFile, newFilePath));
            return newFilePath;
        }
    }
}