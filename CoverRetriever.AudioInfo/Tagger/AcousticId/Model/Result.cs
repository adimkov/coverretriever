namespace CoverRetriever.AudioInfo.Tagger.AcousticId
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    internal class Result
    {
        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("releases")]
        public IList<Release> Releases { get; set; }
    }
}
