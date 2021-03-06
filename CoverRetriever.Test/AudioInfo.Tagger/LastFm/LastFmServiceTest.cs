﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastFmServiceTest.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Tests for LastFmService
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Test.AudioInfo.Tagger.LastFm
{
    using System.Configuration;
    using System.Linq;
    using System.Reactive;
    using System.Xml.Linq;

    using CoverRetriever.AudioInfo.Tagger.LastFm;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    /// <summary>
    /// Tests for LastFmService.
    /// </summary>
    [TestFixture]
    public class LastFmServiceTest
    {
        private LastFmService _lastFmService;
 
        [SetUp]
        public void Given()
        {
            _lastFmService = new LastFmService(
                ConfigurationManager.AppSettings["lastfm.api.address"],
                ConfigurationManager.AppSettings["lastfm.api.key"]);
        }

        [Test]
        public void Should_get_track_info_for_PinkFloyd_TheFinalCut()
        {
            var testSheduler = new TestScheduler();
            var observer = testSheduler.CreateObserver<XDocument>();
            _lastFmService.GetTrackInfo("Pink Floyd", "The final cut").Run(observer);
            testSheduler.Start();
            Assert.That(observer.Messages[0].Value.Kind, Is.EqualTo(NotificationKind.OnNext));
            Assert.That(observer.Messages[0].Value.Value, Is.Not.Null);
            Assert.That(observer.Messages[1].Value.Kind, Is.EqualTo(NotificationKind.OnCompleted));
        }

        [Test]
        public void Should_get_album_info_for_PinkFloyd_TheWall()
        {
            var testSheduler = new TestScheduler();
            var observer = testSheduler.CreateObserver<XDocument>();
            _lastFmService.GetAlbumInfo("Pink Floyd", "Animals").Run(observer);
            testSheduler.Start();
            Assert.That(observer.Messages[0].Value.Kind, Is.EqualTo(NotificationKind.OnNext));
            Assert.That(observer.Messages[0].Value.Value, Is.Not.Null);
            Assert.That(observer.Messages[1].Value.Kind, Is.EqualTo(NotificationKind.OnCompleted));
        }
    }
}