using System.Text.Json.Serialization; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class EditSession
    {
        [JsonPropertyName("has_watermark")]
        public string HasWatermark { get; set; }

        [JsonPropertyName("is_max_resolution")]
        public string IsMaxResolution { get; set; }

        [JsonPropertyName("is_music_licensed")]
        public string IsMusicLicensed { get; set; }

        [JsonPropertyName("is_rated")]
        public bool IsRated { get; set; }

        [JsonPropertyName("min_tier_for_movie")]
        public string MinTierForMovie { get; set; }

        [JsonPropertyName("result_video_hash")]
        public string ResultVideoHash { get; set; }

        [JsonPropertyName("send_transcoding_complete_email")]
        public string SendTranscodingCompleteEmail { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("vsid")]
        public string Vsid { get; set; }
    }

}