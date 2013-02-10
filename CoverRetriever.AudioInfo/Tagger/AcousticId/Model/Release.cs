namespace CoverRetriever.AudioInfo.Tagger.AcousticId
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    internal class Release
    {
        [JsonProperty("track_count")]
        public int TrackCount { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("artists")]
        public IList<Artist> Artists { get; set; }

        [JsonProperty("date")]
        public Date Date { get; set; }

        [JsonProperty("medium_count")]
        public int MediumCount { get; set; }

        [JsonProperty("mediums")]
        public IList<Medium> Mediums { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
