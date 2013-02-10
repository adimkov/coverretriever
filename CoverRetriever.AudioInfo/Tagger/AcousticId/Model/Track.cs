namespace CoverRetriever.AudioInfo.Tagger.AcousticId
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    internal class Track
    {

        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("artists")]
        public IList<Artist> Artists { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
