using System.Text.Json.Serialization; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class EndScreen
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

}