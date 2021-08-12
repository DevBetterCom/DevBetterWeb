using System.Text.Json.Serialization; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class Website
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }

}