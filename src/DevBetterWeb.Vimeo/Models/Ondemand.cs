using System.Text.Json.Serialization; 
using System.Collections.Generic; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class Ondemand
    {
        [JsonPropertyName("options")]
        public List<string> Options { get; set; }

        [JsonPropertyName("resource_key")]
        public string ResourceKey { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }

}