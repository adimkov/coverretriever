namespace CoverRetriever.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Remote cover that cache downloaded result in memory
    /// </summary>
    /// <remarks>
    /// If cache does not used for some time, cache will be removed and downloaded again.
    /// Class thread safe
    /// </remarks>
    public class CachedRemoteCover : RemoteCover
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
        /// indicating is cover in progress of downloading
        /// </summary>
        private bool _isDownloading;

        /// <summary>
        /// Cached cover.
        /// </summary>
        private byte[] _downloadedCover;

        /// <summary>
        /// Download cover exception
        /// </summary>
        private Exception _lastException;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedRemoteCover"/> class.
        /// </summary>
        /// <param name="coverStream">The cover stream.</param>
        public CachedRemoteCover(IObservable<Stream> coverStream)
        {
            _sourceStream = coverStream;
        }

        /// <summary>
        /// Gets cached cover stream.
        /// </summary>
        /// <remarks>Creates new stream every time</remarks>
        public override IObservable<Stream> CoverStream
        {
            get
            {
                if (_downloadedCover != null)
                {
                    return Observable.Return(ProduceCoverStream());
                }

                if (_lastException != null)
                {
                    return Observable.Throw<Stream>(_lastException);
                }

                if (!_isDownloading)
                {
                    _isDownloading = true;
                        
                    return _sourceStream.Do(
                        x =>
                        {
                            _downloadedCover = new byte[x.Length];
                            x.Read(_downloadedCover, 0, _downloadedCover.Length);

                            _coverStreamListeners.ForEach(s => s.OnNext(ProduceCoverStream()));
                        },
                        ex =>
                        {
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