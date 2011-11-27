// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackInfoResponseParserTest.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Test for TrackInfoResponseParser
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Test.AudioInfo.Tagger.LastFm
{
    using System.Xml.Linq;

    using CoverRetriever.AudioInfo.Tagger.LastFm;

    using NUnit.Framework;

    [TestFixture]
    public class TrackInfoResponseParserTest
    {
        [Test]
        public void Should_parse_valid_TaskInfoResponse_and_return_albums()
        {
            using (var responseStream = ResourceUtils.GetTestInputStream("ValidTaskInfoResponse.xml"))
            {
                var response = XDocument.Load(responseStream);
                var target = new TrackInfoResponseParser();
                target.Parse(response);

                CollectionAssert.AreEquivalent(new[] { "Oh By the Way (Studio Album Boxset)" }, target.SuggestedAlbums);
            }
        }
    }
}