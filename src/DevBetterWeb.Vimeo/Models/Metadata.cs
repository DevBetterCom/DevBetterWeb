using System.Text.Json.Serialization; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class Metadata
    {
        [JsonPropertyName("connections")]
        public Connections Connections { get; set; }

        [JsonPropertyName("interactions")]
        public Interactions Interactions { get; set; }

        [JsonPropertyName("is_screen_record")]
        public bool IsScreenRecord { get; set; }

        [JsonPropertyName("is_vimeo_create")]
        public bool IsVimeoCreate { get; set; }

        [JsonPropertyName("metadata")]
        public Metadata MetadataData { get; set; }

        [JsonPropertyName("public_videos")]
        public PublicVideos PublicVideos { get; set; }
    }

}
