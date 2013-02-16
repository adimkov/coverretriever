// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FingerprintParser.cs" author="Anton Dimkov">
//     Copyright (c) Anton Dimkov 2013. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.AudioInfo.Tagger.AcousticId
{
    using System;
    using System.Text.RegularExpressions;

    public static class FingerprintParser
    {
        /// <summary>
        /// The duration regex.
        /// </summary>
        private static string durationRegex = "DURATION=(?<duration>\\d+)";

        /// <summary>
        /// The fingerprint regex.
        /// </summary>
        private static string fingerprintRegex = "FINGERPRINT=(?<fingerprint>[0-9a-z_-]+)";

        /// <summary>
        /// Parses the specified fingerprint output.
        /// </summary>
        /// <param name="fingerprintOutput">The fingerprint output.</param>
        /// <returns>Parsed fingerprint.</returns>
        /// <exception cref="System.ArgumentException">Unable to parse file fingerprint</exception>
        public static Fingerprint Parse(string fingerprintOutput)
        {
            var durationMatch = Regex.Match(fingerprintOutput, durationRegex, RegexOptions.IgnoreCase).Groups["duration"];
            var fingerprintMatch = Regex.Match(fingerprintOutput, fingerprintRegex, RegexOptions.IgnoreCase).Groups["fingerprint"];

            if (durationMatch.Success && fingerprintMatch.Success)
            {
                return new Fingerprint(
                    Int32.Parse(durationMatch.Value),
                    fingerprintMatch.Value);    
            }

            throw new ArgumentException("Unable to parse file fingerprint");
        }
    }
}