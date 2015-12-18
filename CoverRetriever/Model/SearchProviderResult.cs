using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoverRetriever.Model
{
    public struct SearchProviderResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchProviderResult"/> struct.
        /// </summary>
        /// <param name="rootFolder">The root folder.</param>
        public SearchProviderResult(string provider)
            : this()
        {
            HasValue = true;
            SearchProvider = provider;
        }

        /// <summary>
        /// Gets a value indicating whether does user chose folder.
        /// </summary>
        public bool HasValue { get; private set; }

        /// <summary>
        /// Gets selected folder.
        /// </summary>
        public string SearchProvider { get; private set; }

    }
}
