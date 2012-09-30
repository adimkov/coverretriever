// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CachedCover.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
//  Cache service for cover.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using CoverRetriever.AudioInfo;

    /// <summary>
    /// Cover that cache downloaded result in memory.
    /// </summary>
    /// <remarks>
    /// If cache does not used for some time, cache will be removed and downloaded again.
    /// Class thread safe.
    /// </remarks>
    public class CachedCover : Cover
    {
        /// <summary>
        /// Subject for listeners of remote cover.
        /// </summary>
        private readonly List<Subject<Stream>> _coverStreamListeners = new List<Subject<Stream>>();

        /// <summary>
        /// The source stream of cover.
        /// </summary>
        private readonly IObservable<Stream> _sourceStream;

        /// <summary>
        /// Indicating is cover in progress of downloading.
        /// </summary>
        private bool _isDownloading;

        /// <summary>
        /// Cached cover.
        /// </summary>
        private byte[] _downloadedCover;

        /// <summary>
        /// Download cover exception.
        /// </summary>
        private Exception _lastException;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedCover"/> class.
        /// </summary>
        /// <param name="sourceCover">The source cover.</param>
        public CachedCover(Cover sourceCover)
        {
            _sourceStream = sourceCover.CoverStream;
            Name = sourceCover.Name;
            CoverSize = sourceCover.CoverSize;
            Length = sourceCover.Length;
        }

        /// <summary>
        /// Gets cached cover stream.
        /// </summary>
        /// <remarks>Creates new stream every time.</remarks>
        public override IObservable<Stream> CoverStream
        {
            get
            {
                if (_downloadedCover != null)
                {
                    Trace.WriteLine("Return cached cover");
                    return Observable.Return(ProduceCoverStream());
                }

                if (_lastException != null)
                {
                    Trace.WriteLine("Return cached error");
                    return Observable.Throw<Stream>(_lastException);
                }

                if (!_isDownloading)
                {
                    Trace.WriteLine("Perform first download cover");
                    _isDownloading = true;
                        
                    return _sourceStream.Do(
                        x =>
                        {
                            Trace.WriteLine("Cover successfully downloaded");
                            _downloadedCover = new byte[x.Length];
                            x.Read(_downloadedCover, 0, _downloadedCover.Length);
                            x.Seek(0, SeekOrigin.Begin); // Return start position back for read by listeners

                            _coverStreamListeners.ForEach(s => s.OnNext(ProduceCoverStream()));
                        },
                        ex =>
                        {
                            Trace.WriteLine("Cover was nod downloaded due error");
                            _lastException = ex;
                            _coverStreamListeners.ForEach(s => s.OnError(ex));
                        }).Finally(
                            () =>
                            {
                                _isDownloading = false;
                                _coverStreamListeners.Clear();
                            });
                }
                
                var listener = new Subject<Stream>();
                _coverStreamListeners.Add(listener);
                return listener;    
            }
        }

        /// <summary>
        /// Produces the cover stream from downloaded cover.
        /// </summary>
        /// <returns>New stream of cover.</returns>
        private MemoryStream ProduceCoverStream()
        {
            return new MemoryStream(_downloadedCover);
        }
    }
}