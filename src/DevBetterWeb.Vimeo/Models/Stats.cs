using System.Text.Json.Serialization; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class Stats
    {
        [JsonPropertyName("plays")]
        public int Plays { get; set; }
    }

}