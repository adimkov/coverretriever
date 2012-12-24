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
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;

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

            return Observable.Defer(
                () =>
                {
                    Observable.Empty<Unit>()
                        .Delay(TimeSpan.FromMilliseconds(5))
                        .Completed(doAction);
                    return observable;
                });
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
            
            source.Subscribe(
               subject.OnNext,
                subject.OnError,
                () =>
                {
                    completeAction();
                    subject.OnCompleted();
                });

            return subject;
        }

        /// <summary>
        /// Runs sync specified source.
        /// </summary>
        /// <typeparam name="T">
        /// Type of observable.
        /// </typeparam>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="observer">
        /// The observer.
        /// </param>
        ////[Obsolete("Foreach method")]
        public static void Run<T>(this IObservable<T> source, IObserver<T> observer)
        {
            var resetEvent = new ManualResetEvent(false);
            
            source
                .Finally(() => resetEvent.Set())
                .Subscribe(observer);
            
            resetEvent.WaitOne();
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
            return
                source.Timestamp().Do(
                    x => Diagnostics.Trace.TraceInformation("OnNext '{0}': [{1}] {2}", name, x.Timestamp, x.Value),
                    ex => Diagnostics.Trace.TraceError("OnError '{0}': {1}", name, ex),
                    () => Diagnostics.Trace.TraceInformation("OnCompleted '{0}'", name)).Select(x => x.Value);
        }
    }
}
