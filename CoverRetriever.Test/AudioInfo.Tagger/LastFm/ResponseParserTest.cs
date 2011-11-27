// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseParserTest.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
//  Test for ResponseParser
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Test.AudioInfo.Tagger.LastFm
{
    using System.Xml.Linq;

    using CoverRetriever.AudioInfo.Tagger.LastFm;

    using NUnit.Framework;

    /// <summary>
    /// Test for ResponseParser.
    /// </summary>
    public class ResponseParserTest
    {
        [TestCase("ValidLastFmFingerprintResponse.xml", TestName = "Should_parse_valid_response", Result = true)]
        [TestCase("EmptyLastFmFingerprintResponse.xml", TestName = "Should_parse_empty_response", Result = true)]
        [TestCase("InvalidLastFmFingerprintResponse.xml", TestName = "Should_parse_invalid_response", Result = false)]
        public bool Should_parse_different_responses(string responceSource)
        {
            using (var responseStream = ResourceUtils.GetTestInputStream(responceSource))
            {
                var response = XDocument.Load(responseStream);
                var target = new BaseResponseParser();
                target.Parse(response);

                return target.IsSuccess;
            }
        }
    }

    public class BaseResponseParser : ResponseParser { }
}