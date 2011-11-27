// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlbumInfoResponseParserTest.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Test.AudioInfo.Tagger.LastFm
{
    using System.Xml.Linq;

    using CoverRetriever.AudioInfo.Tagger.LastFm;

    using NUnit.Framework;

    [TestFixture]
    public class AlbumInfoResponseParserTest
    {
        [Test]
        public void Should_parse_valid_AlbomInfoResponse_and_return_yers()
        {
            using (var responseStream = ResourceUtils.GetTestInputStream("ValidAlbumInfoResponse.xml"))
            {
                var response = XDocument.Load(responseStream);
                var target = new AlbumInfoResponseParser();
                target.Parse(response);

                CollectionAssert.AreEquivalent(new[] { "1977" }, target.SuggestedYears);
            }
        }    
    }
}