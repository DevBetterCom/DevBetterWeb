using System.Text.Json.Serialization; 
using System.Collections.Generic; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class Uploader
    {
        [JsonPropertyName("pictures")]
        public Pictures Pictures { get; set; }
    }

}
