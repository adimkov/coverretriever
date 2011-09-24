// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVersionControlService.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Service to access the application version
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Service
{
    using System;

    using CoverRetriever.Model;

    /// <summary>
    /// Service to access the application version.
    /// </summary>
    public interface IVersionControlService
    {
        /// <summary>
        /// Get latest version description.
        /// </summary>
        /// <returns>Observable of latest application version.</returns>
        IObservable<RevisionVersion> GetLatestVersion();
    }
}