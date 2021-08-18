using System.Text.Json.Serialization; 
using System.Collections.Generic; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class Pictures
    {
        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("default_picture")]
        public bool DefaultPicture { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("resource_key")]
        public string ResourceKey { get; set; }

        [JsonPropertyName("sizes")]
        public List<Size> Sizes { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        [JsonPropertyName("options")]
        public List<string> Options { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

}