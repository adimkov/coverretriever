// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationKeys.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  The key of application configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Common.Infrastructure
{
    /// <summary>
    /// The key of application configuration.
    /// </summary>
    public static class ConfigurationKeys
    {
        /// <summary>
        /// Path to versions history file. 
        /// </summary>  
        public const string VersionControlConnectionString = "VersionControlConnectionString";

        /// <summary>
        /// URI to project home site.
        /// </summary>
        public const string ProjectHomeUri = "ProjectHomeUri";

        /// <summary>
        /// URI to my blog. 
        /// </summary>
        public const string BlogUri = "BlogUri";

        /// <summary>
        /// URI where new version can be reached.
        /// </summary>
        public const string GetNewVersionUri = "GetNewVersionUri";

        /// <summary>
        /// Last fm API key.
        /// </summary>
        public const string LastFmApiKey = "LastFmApiKey";

        /// <summary>
        /// Last fm base address of service.
        /// </summary>
        public const string LastFmServiceBaseAddress = "LastFmServiceBaseAddress";

        /// <summary>
        /// Last fm fingerprint client path.
        /// </summary>
        public const string LastFmFingerprintClientPath = "LastFmFingerprintClientPath";
    }
}