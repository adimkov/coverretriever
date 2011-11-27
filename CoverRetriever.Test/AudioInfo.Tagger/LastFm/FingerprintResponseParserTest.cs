// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastFmFingerprintResponseParser.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Test for parser of fingerprint response from last.fm  
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Test.AudioInfo.Tagger.LastFm
{
    using System.Xml.Linq;

    using CoverRetriever.AudioInfo.Tagger.LastFm;

    using NUnit.Framework;

    [TestFixture]
    public class FingerprintResponseParserTest
    {

        [Test]
        public void Should_parse_empty_response_and_clear_all_data()
        {
            using (var responseStream = ResourceUtils.GetTestInputStream("EmptyLastFmFingerprintResponse.xml"))
            {
                var response = XDocument.Load(responseStream);
                var target = new FingerprintResponseParser();
                target.Parse(response);

                CollectionAssert.IsEmpty(target.SuggestedArtists);
                CollectionAssert.IsEmpty(target.SuggestedTrackNames);
            }
        }

        [Test]
        public void Should_parse_invalid_response_and_clear_all_data()
        {
            using (var responseStream = ResourceUtils.GetTestInputStream("InvalidLastFmFingerprintResponse.xml"))
            {
                var response = XDocument.Load(responseStream);
                var target = new FingerprintResponseParser();
                target.Parse(response);

                CollectionAssert.IsEmpty(target.SuggestedArtists);
                CollectionAssert.IsEmpty(target.SuggestedTrackNames);
            }
        }

        [Test]
        public void Should_parse_valid_response_and_set_SuggestedArtists_and_SuggestedTrackNames()
        {
            using (var responseStream = ResourceUtils.GetTestInputStream("ValidLastFmFingerprintResponse.xml"))
            {
                var response = XDocument.Load(responseStream);
                var target = new FingerprintResponseParser();
                target.Parse(response);

                CollectionAssert.AreEquivalent(new[] { "Поэт", "Poet", "01_POET" }, target.SuggestedTrackNames);
                CollectionAssert.AreEquivalent(new[] { "ДДТ", "005" }, target.SuggestedArtists);
            }
        }
    }
}