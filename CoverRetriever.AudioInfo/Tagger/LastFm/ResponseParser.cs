// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseParser.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Base response parser.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo.Tagger.LastFm
{
    using System;
    using System.Xml.Linq;

    /// <summary>
    /// Base response parser.
    /// </summary>
    public abstract class ResponseParser
    {
        /// <summary>
        /// Status attribute.
        /// </summary>
        private const string StatusAttribute = "status";

        /// <summary>
        /// Ok response status.
        /// </summary>
        private const string OkStatuss = "ok";

        /// <summary>
        /// Gets or sets a value indicating whether response is success.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is success; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccess { get; protected set; }
        
        /// <summary>
        /// Parses the specified response.
        /// </summary>
        /// <param name="response">The response.</param>
        public virtual void Parse(XDocument response)
        {
            IsSuccess = false;
            if (response != null && response.Root != null)
            {
                var statusAttribute = response.Root.Attribute(StatusAttribute);
                if (statusAttribute != null)
                {
                    IsSuccess = String.Equals(
                        statusAttribute.Value, OkStatuss, StringComparison.CurrentCultureIgnoreCase);
                }
            }
        }
    }
}