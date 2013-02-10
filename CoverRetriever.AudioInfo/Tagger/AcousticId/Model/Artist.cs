// JSON C# Class Generator
// http://at-my-window.blogspot.com/?page=json-class-generator
namespace CoverRetriever.AudioInfo.Tagger.AcousticId
{
    using Newtonsoft.Json;

    internal class Artist
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
