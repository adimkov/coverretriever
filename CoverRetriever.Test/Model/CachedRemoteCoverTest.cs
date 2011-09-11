// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CachedRemoteCoverTest.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
// Tests for CachedRemoteCover calss
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Test.Model
{
    using System;
    using System.Concurrency;
    using System.IO;
    using System.Linq;
    using System.Reactive.Testing.Mocks;

    using CoverRetriever.Model;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class CachedRemoteCoverTest
    {
        [Test]
        public void Should_perform_download_one_time_on_two_subscribtion_with_same_result()
        {
            var downloadRequestCount = 0;
            var coverStream = Observable.Create<Stream>(observer =>
            {
                downloadRequestCount++;
                observer.OnNext(new Mock<Stream>().Object);
                observer.OnCompleted();
                return () => {};
            });

            var scheduler = new TestScheduler();
            var mockObserver1 = new MockObserver<Stream>(scheduler);
            var mockObserver2 = new MockObserver<Stream>(scheduler);

            var target = new CachedRemoteCover(coverStream);
           
            target.CoverStream.Subscribe(mockObserver1);
            target.CoverStream.Subscribe(mockObserver2);

            Assert.AreEqual(1, downloadRequestCount);
            Assert.AreEqual(
                StreamExtractor(mockObserver1[0].Value.Value),
                StreamExtractor(mockObserver2[0].Value.Value));
        }

        [Test]
        public void Should_perform_download_one_pass_error_for_all_listeners()
        {
            var downloadRequestCount = 0;
            var coverStream = Observable.Create<Stream>(observer =>
            {
                downloadRequestCount++;
                observer.OnError(new Exception());
                return () => { };
            });

            var scheduler = new TestScheduler();
            var mockObserver1 = new MockObserver<Stream>(scheduler);
            var mockObserver2 = new MockObserver<Stream>(scheduler);

            var target = new CachedRemoteCover(coverStream);

            target.CoverStream.Subscribe(mockObserver1);
            target.CoverStream.Subscribe(mockObserver2);

            Assert.AreEqual(1, downloadRequestCount);
            Assert.NotNull(mockObserver1[0].Value.Exception);
            Assert.NotNull(mockObserver2[0].Value.Exception);
            Assert.AreEqual(mockObserver1[0].Value.Exception, mockObserver2[0].Value.Exception);
    
        }

        /// <summary>
        /// Extractor of stream content.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>
        /// Content of stream
        /// </returns>
        private byte[] StreamExtractor(Stream stream)
        {
            var streamContent = new byte[stream.Length];
            stream.Read(streamContent, 0, streamContent.Length);

            return streamContent;
        }
    }
}