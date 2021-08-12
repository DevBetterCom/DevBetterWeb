using System.Text.Json.Serialization; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class Upload
    {
        [JsonPropertyName("approach")]
        public string Approach { get; set; }

        [JsonPropertyName("complete_uri")]
        public string CompleteUri { get; set; }

        [JsonPropertyName("form")]
        public string Form { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("redirect_url")]
        public string RedirectUrl { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("upload_link")]
        public string UploadLink { get; set; }
    }

}