namespace CoverRetriever.AudioInfo.Tagger.AcousticId
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    internal class AcousticResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("results")]
        public IList<Result> Results { get; set; }
    }
}
