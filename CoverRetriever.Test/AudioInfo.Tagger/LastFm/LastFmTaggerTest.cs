// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastFmTaggerTest.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Tests for LastFmTaggerService class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Test.AudioInfo.Tagger.LastFm
{
    using System.Configuration;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;

    using CoverRetriever.AudioInfo;
    using CoverRetriever.AudioInfo.Tagger.LastFm;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    [TestFixture]
    public class LastFmTaggerTest
    {
        private const string FileToRetrieve = "DDT-Poet.mp3";

        private readonly string LastfmfpclientUtility;
        private readonly string LastfmSericeAddress;
        private readonly string LastfmApiKey;

        private LastFmTaggerService _lastFmTaggerService;

        public LastFmTaggerTest()
        {
            LastfmfpclientUtility = ConfigurationManager.AppSettings["test.folder.lastfmfpclient"];
            LastfmSericeAddress = ConfigurationManager.AppSettings["lastfm.api.address"];
            LastfmApiKey = ConfigurationManager.AppSettings["lastfm.api.key"]; 
        }

        [SetUp]
        public void TestSetUp()
        {
            _lastFmTaggerService = new LastFmTaggerService(LastfmfpclientUtility, LastfmSericeAddress, LastfmApiKey);    
        }

        [Test]
        public void Should_get_tags_for_file_with_empty_farame()
        {
            var suggestedTag = _lastFmTaggerService
                .LoadTagsForAudioFile(PathUtils.BuildFullResourcePath(FileToRetrieve))
                .Single();

            Assert.That(suggestedTag.Artist, Is.EqualTo("ДДТ"));
            Assert.That(suggestedTag.Album, Is.EqualTo("Город без окон. Вход."));
            Assert.That(suggestedTag.TrackName, Is.EqualTo("Поэт"));
//            Assert.That(_lastFmTaggerService.Year, Is.EqualTo(2003));
        }
    }
}
