// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeferredCallExtensions.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Extension method to make deferred call.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Common.Extensions
{
    using System;
    using System.Linq;

    /// <summary>
    /// Extension method to make deferred call.
    /// </summary>
    public static class DeferredCallExtensions
    {
        /// <summary>
        /// Make deferred call to a service.
        /// </summary>
        /// <typeparam name="T">Type of observable.</typeparam>
        /// <param name="observable">The observable.</param>
        /// <param name="doAction">Call service action.</param>
        /// <returns>Defer observable.</returns>
        public static IObservable<T> Defer<T>(this IObservable<T> observable, Action doAction)
        {
            var actionObservable = Observable.Timer(TimeSpan.FromMilliseconds(1)).Do(_ => doAction());

            return Observable.Empty<T>().Do(x => { }, () => actionObservable.Subscribe()).Concat(observable);
        }
    }
}