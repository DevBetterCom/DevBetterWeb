using System.Text.Json.Serialization; 
using System.Collections.Generic; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class Report
    {
        [JsonPropertyName("options")]
        public List<string> Options { get; set; }

        [JsonPropertyName("reason")]
        public List<string> Reason { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }

}