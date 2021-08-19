using System.Text.Json.Serialization; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class Log
    {
        [JsonPropertyName("play")]
        public string Play { get; set; }
    }

}