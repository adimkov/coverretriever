namespace CoverRetriever.AudioInfo.Tagger.AcousticId
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    internal class Medium
    {
        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("tracks")]
        public IList<Track> Tracks { get; set; }

        [JsonProperty("track_count")]
        public int TrackCount { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }
    }
}
