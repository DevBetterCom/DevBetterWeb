using System.Text.Json.Serialization; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class Space
    {
        [JsonPropertyName("free")]
        public int Free { get; set; }

        [JsonPropertyName("max")]
        public long Max { get; set; }

        [JsonPropertyName("showing")]
        public string Showing { get; set; }

        [JsonPropertyName("used")]
        public long Used { get; set; }
    }

}