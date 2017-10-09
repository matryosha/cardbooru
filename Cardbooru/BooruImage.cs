using Newtonsoft.Json;

namespace Cardbooru
{
    public class BooruImage {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("preview_file_url")]
        public string PreviewUrl { get; set; }

        [JsonProperty("large_file_url")]
        public string ImageUrl { get; set; }

        [JsonProperty("md5")]
        public string Hash { get; set; }


    }
}
