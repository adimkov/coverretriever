// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastFmTaggerTest.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Tests for LastFmTagger class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Test.AudioInfo.Tagger
{
    using System.Configuration;

    using CoverRetriever.AudioInfo.Tagger;

    using NUnit.Framework;

    [TestFixture]
    public class LastFmTaggerTest
    {
        private const string FileWithEmptyFarame = "DDT-Poet.mp3";

        private readonly string LastfmfpclientUtility;

        private LastFmTagger _lastFmTagger;

        public LastFmTaggerTest()
        {
            LastfmfpclientUtility = ConfigurationManager.AppSettings["test.folder.lastfmfpclient"]; 
        }

        [SetUp]
        public void TestSetUp()
        {
            _lastFmTagger = new LastFmTagger(LastfmfpclientUtility);    
        }

        [Test]
        public void Should_get_tags_for_file_with_empty_farame()
        {
//            _lastFmTagger.GetTagsForAudioFile(PathUtils.BuildFullResourcePath(FileWithEmptyFarame));
            _lastFmTagger.GetTagsForAudioFile(@"d:\Music\Pain\Pain - 0.mp3");
        }
    }
}