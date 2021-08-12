using System.Text.Json.Serialization; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class UploadAttempt
    {
        [JsonPropertyName("clip")]
        public Clip Clip { get; set; }

        [JsonPropertyName("complete_uri")]
        public string CompleteUri { get; set; }

        [JsonPropertyName("form")]
        public string Form { get; set; }

        [JsonPropertyName("ticket_id")]
        public string TicketId { get; set; }

        [JsonPropertyName("upload_link")]
        public string UploadLink { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; }
    }

}
