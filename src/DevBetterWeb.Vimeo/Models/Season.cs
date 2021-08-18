using System.Text.Json.Serialization; 
using System.Collections.Generic; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class Season
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("options")]
        public List<string> Options { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }

}