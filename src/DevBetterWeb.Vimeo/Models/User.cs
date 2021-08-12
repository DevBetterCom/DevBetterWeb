using System.Text.Json.Serialization; 
using System.Collections.Generic; 
using System; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class User
    {
        [JsonPropertyName("account")]
        public string Account { get; set; }

        [JsonPropertyName("available_for_hire")]
        public bool AvailableForHire { get; set; }

        [JsonPropertyName("bio")]
        public string Bio { get; set; }

        [JsonPropertyName("can_work_remotely")]
        public bool CanWorkRemotely { get; set; }

        [JsonPropertyName("capabilities")]
        public List<object> Capabilities { get; set; }

        [JsonPropertyName("clients")]
        public string Clients { get; set; }

        [JsonPropertyName("content_filter")]
        public List<object> ContentFilter { get; set; }

        [JsonPropertyName("created_time")]
        public DateTime CreatedTime { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("location_details")]
        public List<LocationDetail> LocationDetails { get; set; }

        [JsonPropertyName("metadata")]
        public Metadata Metadata { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("pictures")]
        public List<object> Pictures { get; set; }

        [JsonPropertyName("preferences")]
        public Preferences Preferences { get; set; }

        [JsonPropertyName("resource_key")]
        public string ResourceKey { get; set; }

        [JsonPropertyName("short_bio")]
        public string ShortBio { get; set; }

        [JsonPropertyName("skills")]
        public Skills Skills { get; set; }

        [JsonPropertyName("upload_quota")]
        public UploadQuota UploadQuota { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        [JsonPropertyName("websites")]
        public List<Website> Websites { get; set; }
    }

}