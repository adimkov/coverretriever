// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableExtensions.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Extension method to make deferred call.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace System.Linq
{
    using System;
    using System.Collections.Generic;

    using CoverRetriever.Common.Validation;

    /// <summary>
    /// Extension method to make deferred call.
    /// </summary>
    public static class ObservableExtensions
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
            Require.NotNull(observable, "observable");
            Require.NotNull(doAction, "doAction");

            var actionObservable = Observable.Timer(TimeSpan.FromMilliseconds(1)).Do(_ => doAction());

            return Observable.Empty<T>().Do(x => { }, () => actionObservable.Subscribe()).Concat(observable);
        }

        /// <summary>
        /// Completes the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="completeAction">The complete action.</param>
        /// <returns>The Observable.</returns>
        public static IObservable<TSource> Completed<TSource>(this IObservable<TSource> source, Action completeAction)
        {
            Require.NotNull(source, "source");
            Require.NotNull(completeAction, "completeAction");

            var subject = new Subject<TSource>();

            return subject.Defer(
                () => source.Subscribe(
                    subject.OnNext,
                    subject.OnError,
                    () =>
                    {
                        completeAction();
                        subject.OnCompleted();
                    }));
        }

        /// <summary>
        /// Traces the specified observable.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="name">The name.</param>
        /// <returns>The source observable.</returns>
        public static IObservable<TSource> Trace<TSource>(this IObservable<TSource> source, string name)
        {
            return source
                .Timestamp()
                .Do(
                    x => Diagnostics.Trace.TraceInformation("OnNext '{0}': [{1}] {2}", name, x.Timestamp, x.Value),
                    ex => Diagnostics.Trace.TraceError("OnError '{0}': {1}", name, ex),
                    () => Diagnostics.Trace.TraceInformation("OnCompleted '{0}'", name))
                .RemoveTimestamp();
        }
    }
}