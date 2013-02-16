// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AcousticResponseHelperTest.cs" author="Anton Dimkov">
//     Copyright (c) Anton Dimkov 2013. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Test.AudioInfo.Tagger.AcousticId
{
    using System.IO;

    using CoverRetriever.AudioInfo.Tagger.AcousticId;
    using CoverRetriever.Test;

    using FluentAssertions;

    using NUnit.Framework;

    using Newtonsoft.Json;

    [TestFixture]
    public class AcousticResponseHelperTest
    {
        [Test]
        public void Should_aggregate_artist_name_from_response()
        {
            using (var responce = ResourceUtils.GetTestInputStream("AcousticId_AcDc_Response.txt"))
            using (var responceReader = new StreamReader(responce))
            {
                var artist = AcousticResponseHelper.AggrigateArtist(
                    JsonConvert.DeserializeObject<AcousticResponse>(responceReader.ReadToEnd()));

                artist.Should().Be("AC/DC");
            }
        }

        [Test]
        public void Should_aggregate_track_from_response()
        {
            using (var responce = ResourceUtils.GetTestInputStream("AcousticId_AcDc_Response.txt"))
            using (var responceReader = new StreamReader(responce))
            {
                var artist = AcousticResponseHelper.AggrigateTrackName(
                    JsonConvert.DeserializeObject<AcousticResponse>(responceReader.ReadToEnd()),
                    "AC/DC");

                artist.Should().Be("Dirty Deeds Done Dirt Cheap");
            }
        }

        [Test]
        public void Should_aggregate_album_name_from_response()
        {
            using (var responce = ResourceUtils.GetTestInputStream("AcousticId_AcDc_Response.txt"))
            using (var responceReader = new StreamReader(responce))
            {
                var album = AcousticResponseHelper.AggrigateAlbum(
                    JsonConvert.DeserializeObject<AcousticResponse>(responceReader.ReadToEnd()),
                    "AC/DC",
                    "Dirty Deeds Done Dirt Cheap");

                album.Should().Be("Dirty Deeds Done Dirt Cheap");
            }
        }

        [Test]
        public void Should_aggregate_year_from_response()
        {
            using (var responce = ResourceUtils.GetTestInputStream("AcousticId_AcDc_Response.txt"))
            using (var responceReader = new StreamReader(responce))
            {
                var year = AcousticResponseHelper.AggrigateYear(
                    JsonConvert.DeserializeObject<AcousticResponse>(responceReader.ReadToEnd()),
                    "AC/DC",
                    "Dirty Deeds Done Dirt Cheap",
                    "Dirty Deeds Done Dirt Cheap");

                year.Should().Be(2003);
            }
        }     
    }
}