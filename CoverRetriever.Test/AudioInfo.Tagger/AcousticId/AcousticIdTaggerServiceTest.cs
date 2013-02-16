// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AcousticIdTaggerServiceTest.cs" author="Anton Dimkov">
//     Copyright (c) Anton Dimkov 2013. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------------------------

namespace CoverRetriever.Test.AudioInfo.Tagger.AcousticId
{
    using System.Reactive.Linq;

    using CoverRetriever.AudioInfo.Tagger.AcousticId;

    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class AcousticIdTaggerServiceTest
    {
        private const string FileToRetrieve = "DDT-Poet.mp3";

        [Test]
        public void Should_retreive_tags_for_ddt_poet_mp3()
        {
            var suggestedTag = new AcousticIdTaggerService("rAFaRxdy", "./AcoustId/fpcalc.exe")
                .LoadTagsForAudioFile(PathUtils.BuildFullResourcePath(FileToRetrieve))
                .Single();

            suggestedTag.Artist.Should().Be("ДДТ");
            suggestedTag.Album.Should().Be("Я получил эту роль");
            suggestedTag.Year.Should().Be("1989");
            suggestedTag.TrackName.Should().Be("Поэт");
        }
    }
}
