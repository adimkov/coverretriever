// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FingerprintParserTest.cs" author="Anton Dimkov">
//     Copyright (c) Anton Dimkov 2013. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Test.AudioInfo.Tagger.AcousticId
{
    using System.IO;

    using CoverRetriever.AudioInfo.Tagger.AcousticId;

    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class FingerprintParserTest
    {
        [Test]
        public void Should_parse_fingerpint_ulility_output()
        {
            using (var fingerprintFull = ResourceUtils.GetTestInputStream("AcDc_fingerprint_full.txt"))
            using (var fingerprintExpected = ResourceUtils.GetTestInputStream("AcDc_Fingerprint.txt"))
            using (var readerFull = new StreamReader(fingerprintFull))
            using (var readerExpected = new StreamReader(fingerprintExpected))
            {
                var parser = FingerprintParser.Parse(readerFull.ReadToEnd());

                parser.Duration.Should().Be(252);
                parser.FingerprintString.Should().Be(readerExpected.ReadToEnd());
            }
        }
    }
}