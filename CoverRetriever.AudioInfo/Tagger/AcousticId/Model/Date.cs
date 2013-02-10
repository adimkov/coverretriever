namespace CoverRetriever.AudioInfo.Tagger.AcousticId
{
    using Newtonsoft.Json;

    internal class Date
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("month")]
        public int? Month { get; set; }

        [JsonProperty("day")]
        public int? Day { get; set; }
    }
}
