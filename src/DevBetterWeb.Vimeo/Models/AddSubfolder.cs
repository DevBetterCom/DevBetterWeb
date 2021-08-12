using System.Text.Json.Serialization; 
using System.Collections.Generic; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class AddSubfolder
    {
        [JsonPropertyName("can_add_subfolders")]
        public string CanAddSubfolders { get; set; }

        [JsonPropertyName("content_type")]
        public string ContentType { get; set; }

        [JsonPropertyName("options")]
        public List<string> Options { get; set; }

        [JsonPropertyName("properties")]
        public List<string> Properties { get; set; }

        [JsonPropertyName("subfolder_depth_limit_reached")]
        public string SubfolderDepthLimitReached { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }

}