// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Fingerprint.cs" author="Anton Dimkov">
//     Copyright (c) Anton Dimkov 2013. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.AudioInfo.Tagger.AcousticId
{
    /// <summary>
    /// Declaration of the <see cref="Fingerprint" /> entity.
    /// </summary>
    public class Fingerprint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Fingerprint" /> class.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="fingerprintString">The fingerprint string.</param>
        public Fingerprint(int duration, string fingerprintString)
        {
            Duration = duration;
            FingerprintString = fingerprintString;
        }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the fingerprint string.
        /// </summary>
        /// <value>
        /// The fingerprint string.
        /// </value>
        public string FingerprintString { get; set; }
    }
}