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
        /// indicating is cover in progress of downloading
        /// </summary>
        private bool _isDownloading;
        
        /// <summary>
        /// The source stream of cover.
        /// </summary>
        private IObservable<Stream> _sourceStream;

        /// <summary>
        /// Cached cover.
        /// </summary>
        private byte[] _downloadedCover;

        /// <summary>
        /// Subject for listeners of remote cover.
        /// </summary>
        private Subject<Stream> _coverStreamSubject = new Subject<Stream>();

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
                if (!_isDownloading && _downloadedCover == null && _lastException == null)
                {
                    _isDownloading = true;

                    _sourceStream.Do(
                        stream =>
                        {
                            _downloadedCover = new byte[stream.Length];
                            stream.Read(_downloadedCover, 0, _downloadedCover.Length);

                            RaiseCoverDownloaded();
                        },
                        ex =>
                        {
                            _lastException = ex;
                            RaiseErrorDownload();
                        },
                        () => _isDownloading = false).Subscribe();
                }
                else
                {
                    if (_downloadedCover != null)
                    {
                        DeferedRaiseCoverDownloaded();
                    }
                    else if (_lastException != null)
                    {
                        DeferedRaiseErrorDownload();
                    }
                }

                return _coverStreamSubject;
            }
        }

        /// <summary>
        /// Produces the cover stream from downloaded cover.
        /// </summary>
        /// <returns>New stream of cover.</returns>
        private MemoryStream ProduceCoverStream()
        {
            return new MemoryStream(this._downloadedCover);
        }

        /// <summary>
        /// Raises the cover downloaded.
        /// </summary>
        private void RaiseCoverDownloaded()
        {
            _coverStreamSubject.OnNext(ProduceCoverStream());
            _coverStreamSubject.OnCompleted();
        }

        /// <summary>
        /// Defer raises cover downloaded.
        /// </summary>
        private void DeferedRaiseCoverDownloaded()
        {
            _coverStreamSubject.Defer(RaiseCoverDownloaded);
        }

        /// <summary>
        /// Raises the error download.
        /// </summary>
        private void RaiseErrorDownload()
        {
            _coverStreamSubject.OnError(_lastException);
        }

        /// <summary>
        /// Defer raises cover download error.
        /// </summary>
        private void DeferedRaiseErrorDownload()
        {
            _coverStreamSubject.Defer(RaiseErrorDownload);
        }
    }
}