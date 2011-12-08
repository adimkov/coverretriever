// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastFmTaggerTest.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Tests for LastFmTagger class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Test.AudioInfo.Tagger.LastFm
{
    using System;
    using System.Concurrency;
    using System.Configuration;
    using System.Linq;
    using System.Reactive.Testing.Mocks;

    using CoverRetriever.AudioInfo.Tagger.LastFm;

    using NUnit.Framework;

    [TestFixture]
    public class LastFmTaggerTest
    {
        private const string FileToRetrieve = "DDT-Poet.mp3";

        private readonly string LastfmfpclientUtility;
        private readonly string LastfmSericeAddress;
        private readonly string LastfmApiKey;

        private LastFmTagger _lastFmTagger;

        public LastFmTaggerTest()
        {
            LastfmfpclientUtility = ConfigurationManager.AppSettings["test.folder.lastfmfpclient"];
            LastfmSericeAddress = ConfigurationManager.AppSettings["lastfm.api.address"];
            LastfmApiKey = ConfigurationManager.AppSettings["lastfm.api.key"]; 
        }

        [SetUp]
        public void TestSetUp()
        {
            _lastFmTagger = new LastFmTagger(LastfmfpclientUtility, LastfmSericeAddress, LastfmApiKey);    
        }

        [Test]
        public void Should_get_tags_for_file_with_empty_farame()
        {
            var testSheduler = new TestScheduler();
            var observer = new MockObserver<Unit>(testSheduler);

            _lastFmTagger.LoadTagsForAudioFile(PathUtils.BuildFullResourcePath(FileToRetrieve)).Run(observer);

            Assert.That(_lastFmTagger.Artist, Is.EqualTo("ДДТ"));
            Assert.That(_lastFmTagger.Album, Is.EqualTo("Город без окон. Вход."));
            Assert.That(_lastFmTagger.TrackName, Is.EqualTo("Поэт"));
        }
    }
}