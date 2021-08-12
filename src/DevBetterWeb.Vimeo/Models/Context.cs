using System.Text.Json.Serialization; 
using System.Collections.Generic; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class Context
    {
        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("resource")]
        public List<object> Resource { get; set; }

        [JsonPropertyName("resource_type")]
        public string ResourceType { get; set; }
    }

}