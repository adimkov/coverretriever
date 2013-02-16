// -------------------------------------------------------------------------------------------------
// <copyright file="AcousticResponseHelper.cs" author="Anton Dimkov">
//     Copyright (c) Anton Dimkov 2013. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------
namespace CoverRetriever.AudioInfo.Tagger.AcousticId
{
    using System.Linq;

    /// <summary>
    /// Declaration of the <see cref="AcousticResponseHelper" /> class.
    /// </summary>
    internal static class AcousticResponseHelper
    {
        /// <summary>
        /// Averages the artist.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>Average artist name.</returns>
        public static string AggrigateArtist(AcousticResponse response)
        {
            var artists =
                response.Results.SelectMany(x => x.Releases)
                        .SelectMany(x2 => x2.Artists)
                        .Select(x => x.Name)
                        .GroupBy(x => x)
                        .OrderByDescending(x => x.Count())
                        .Select(x => x.Key);

            return artists.FirstOrDefault();
        }

        /// <summary>
        /// Averages the Track name.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="artist">The artist.</param>
        /// <returns>
        /// Average artist name.
        /// </returns>
        public static string AggrigateTrackName(AcousticResponse response, string artist)
        {
            var tracks =
                response.Results.SelectMany(x => x.Releases)
                        .Where(x => x.Artists.Any(a => a.Name == artist))
                        .SelectMany(x => x.Mediums)
                        .SelectMany(x => x.Tracks)
                        .Select(x => x.Title)
                        .GroupBy(x => x)
                        .OrderByDescending(x => x.Count())
                        .Select(x => x.Key);

            return tracks.FirstOrDefault();
        }

        /// <summary>
        /// Averages the Album.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="artist">The artist.</param>
        /// <param name="track">The track.</param>
        /// <returns>
        /// Average album name.
        /// </returns>
        public static string AggrigateAlbum(AcousticResponse response, string artist, string track)
        {
            var artists =
                response.Results.SelectMany(x => x.Releases)
                        .Where(x => x.Artists.Any(a => a.Name == artist))
                        .Where(x => x.Mediums.SelectMany(m => m.Tracks).Any(t => t.Title == track))
                        .Select(x2 => x2.Title)
                        .GroupBy(x => x)
                        .OrderByDescending(x => x.Count())
                        .Select(x => x.Key);

            return artists.FirstOrDefault();
        }

        /// <summary>
        /// Averages the Album.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="artist">The artist.</param>
        /// <param name="album">The album.</param>
        /// <param name="track">The track.</param>
        /// <returns>
        /// Average album name.
        /// </returns>
        public static int AggrigateYear(AcousticResponse response, string artist, string album, string track)
        {
            var year =
                response.Results.SelectMany(x => x.Releases)
                        .Where(x => x.Artists.Any(a => a.Name == artist))
                        .Where(x => x.Title == album)
                        .Where(x => x.Mediums.SelectMany(m => m.Tracks).Any(t => t.Title == track))
                        .Where(x => x.Date != null)
                        .Select(x => x.Date.Year)
                        .GroupBy(x => x)
                        .OrderByDescending(x => x.Count())
                        .Select(x => x.Key);

            return year.FirstOrDefault();
        }
    }
}