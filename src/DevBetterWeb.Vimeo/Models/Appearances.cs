using System.Text.Json.Serialization; 
using System.Collections.Generic; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class Appearances
    {
        [JsonPropertyName("options")]
        public List<string> Options { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }

}