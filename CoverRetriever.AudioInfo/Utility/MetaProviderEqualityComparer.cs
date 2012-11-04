// -----------------------------------------------------------------------------------------------
// <copyright file="MetaProviderEqualityComparer.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Equality comparer for IMetaProvider
// </summary>
// -----------------------------------------------------------------------------------------------

namespace CoverRetriever.AudioInfo.Utility
{
    using System.Collections.Generic;

    /// <summary>
    /// Equality comparer for IMetaProvider.
    /// </summary>
    public class MetaProviderEqualityComparer : IEqualityComparer<IMetaProvider>
    {
        /// <summary>
        /// Test for equals of two meta providers.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns><c>True</c> if meta providers are equal.</returns>
        public bool Equals(IMetaProvider first, IMetaProvider second)
        {
            return 
                first.Album == second.Album &&
                first.Artist == second.Artist &&
                first.TrackName == second.TrackName &&
                first.Year == second.Year;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="metaProvider">The metaProvider.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(IMetaProvider metaProvider)
        {
            int hashCode = metaProvider.Album.GetHashCode();
            hashCode = (hashCode * 397) ^ metaProvider.Artist.GetHashCode();
            hashCode = (hashCode * 397) ^ metaProvider.TrackName.GetHashCode();
            hashCode = (hashCode * 397) ^ metaProvider.Year.GetHashCode();

            return hashCode;
        }
    }
}