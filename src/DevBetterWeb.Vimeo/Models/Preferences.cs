using System.Text.Json.Serialization; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class Preferences
    {
        [JsonPropertyName("videos")]
        public Videos Videos { get; set; }
    }

}