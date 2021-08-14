using System.Text.Json.Serialization; 
using System.Collections.Generic; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class PermissionPolicy
    {
        [JsonPropertyName("created_on")]
        public string CreatedOn { get; set; }

        [JsonPropertyName("modified_on")]
        public string ModifiedOn { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("permission_actions")]
        public List<object> PermissionActions { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }

}
