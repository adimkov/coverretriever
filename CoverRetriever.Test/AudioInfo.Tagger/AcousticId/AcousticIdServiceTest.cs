// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AcousticIdServiceTest.cs" author="Anton Dimkov">
//     Copyright (c) Anton Dimkov 2013. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Test.AudioInfo.Tagger.AcousticId
{
    using System.IO;
    using System.Reactive.Linq;

    using CoverRetriever.AudioInfo.Tagger.AcousticId;

    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class AcousticIdServiceTest
    {
        [Test]
        public void Should_return_acdc_tags_by_fingerprint()
        {
            var service = new AcousticIdService("rAFaRxdy");
            using (var fingerprint = ResourceUtils.GetTestInputStream("AcDc_Fingerprint.txt"))
            using (var reader = new StreamReader((fingerprint)))
            {
                service.Lookup(252, reader.ReadToEnd())
                    .ForEach(x =>
                        {
                            x.Status.Should().Be("ok");
                            x.Results.Count.Should().Be(2);
                            x.Results[0].Id.Should().Be("b890261c-2e22-47f4-a21d-beac2eb19096");
                            x.Results[1].Id.Should().Be("cd8e6d8d-73b9-4f51-b5db-da0bed6431fe");
                        });
            }
        }     
    }
}