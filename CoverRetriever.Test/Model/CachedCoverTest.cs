// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CachedCoverTest.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.  
// </copyright>
// <summary>
// Tests for CachedCover class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Test.Model
{
    using System;
    using System.Concurrency;
    using System.IO;
    using System.Linq;
    using System.Reactive.Testing.Mocks;
    using System.Windows;

    using CoverRetriever.AudioInfo;
    using CoverRetriever.Model;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class CachedCoverTest
    {
        [Test]
        public void Should_create_cached_cover()
        {
            var coverStream = Observable.Return(new Mock<Stream>().Object);
            var source = this.GenerateCoverStub(coverStream);
            var target = new CachedCover(source);

            Assert.AreEqual(source.Name, target.Name);
            Assert.AreEqual(source.CoverSize, target.CoverSize);
            Assert.AreEqual(source.Length, target.Length);
            Assert.NotNull(target.CoverStream);
        }

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

            var target = new CachedCover(GenerateCoverStub(coverStream));
           
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

            var target = new CachedCover(GenerateCoverStub(coverStream));

            target.CoverStream.Subscribe(mockObserver1);
            target.CoverStream.Subscribe(mockObserver2);

            Assert.AreEqual(1, downloadRequestCount);
            Assert.NotNull(mockObserver1[0].Value.Exception);
            Assert.NotNull(mockObserver2[0].Value.Exception);
            Assert.AreEqual(mockObserver1[0].Value.Exception, mockObserver2[0].Value.Exception);
    
        }

        [Test]
        public void Should_pass_downloaded_cover_to_each_listener()
        {
            var testBytes = new byte[2];
            testBytes[0] = 1;
            testBytes[1] = 2;

            var coverStream = Observable.Create<Stream>(observer =>
            {
                observer.OnNext(new MemoryStream(testBytes));
                observer.OnCompleted();
                return () => {};
            });

            var scheduler = new TestScheduler();
            var mockObserver1 = new MockObserver<Stream>(scheduler);
            var mockObserver2 = new MockObserver<Stream>(scheduler);

            var target = new CachedCover(GenerateCoverStub(coverStream));
           
            target.CoverStream.Subscribe(mockObserver1);
            target.CoverStream.Subscribe(mockObserver2);

            CollectionAssert.AreEquivalent(testBytes, StreamExtractor(mockObserver1[0].Value.Value));
            CollectionAssert.AreEquivalent(testBytes, StreamExtractor(mockObserver2[0].Value.Value));
               
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

        /// <summary>
        /// Generates the cover stub.
        /// </summary>
        /// <param name="coverStream">The cover stream.</param>
        /// <returns>The stub</returns>
        private Cover GenerateCoverStub(IObservable<Stream> coverStream)
        {
            return new Cover("TestCover", new Size(300, 300), 1024, coverStream);
        }
    }
}