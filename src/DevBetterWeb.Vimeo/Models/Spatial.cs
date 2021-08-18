using System.Text.Json.Serialization; 
using System.Collections.Generic; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class Spatial
    {
        [JsonPropertyName("director_timeline")]
        public List<DirectorTimeline> DirectorTimeline { get; set; }

        [JsonPropertyName("field_of_view")]
        public int FieldOfView { get; set; }

        [JsonPropertyName("projection")]
        public string Projection { get; set; }

        [JsonPropertyName("stereo_format")]
        public string StereoFormat { get; set; }
    }

}