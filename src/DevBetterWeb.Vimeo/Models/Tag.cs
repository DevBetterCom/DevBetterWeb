using System.Text.Json.Serialization; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class Tag
    {
        [JsonPropertyName("canonical")]
        public string Canonical { get; set; }

        [JsonPropertyName("metadata")]
        public Metadata Metadata { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("resource_key")]
        public string ResourceKey { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }

}