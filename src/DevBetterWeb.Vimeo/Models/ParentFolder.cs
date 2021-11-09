using System; 
using System.Collections.Generic; 
using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class ParentFolder
{
  [JsonPropertyName("options")]
  public List<string> Options { get; set; }

  [JsonPropertyName("uri")]
  public string Uri { get; set; }

  [JsonPropertyName("access_grant")]
  public AccessGrant AccessGrant { get; set; }

  [JsonPropertyName("created_time")]
  public DateTime CreatedTime { get; set; }

  [JsonPropertyName("is_pinned")]
  public bool IsPinned { get; set; }

  [JsonPropertyName("last_user_action_event_date")]
  public DateTime LastUserActionEventDate { get; set; }

  [JsonPropertyName("link")]
  public string Link { get; set; }

  [JsonPropertyName("metadata")]
  public Metadata Metadata { get; set; }

  [JsonPropertyName("modified_time")]
  public DateTime ModifiedTime { get; set; }

  [JsonPropertyName("name")]
  public string Name { get; set; }

  [JsonPropertyName("pinned_on")]
  public DateTime PinnedOn { get; set; }

  [JsonPropertyName("privacy")]
  public Privacy Privacy { get; set; }

  [JsonPropertyName("resource_key")]
  public string ResourceKey { get; set; }

  [JsonPropertyName("user")]
  public User User { get; set; }
}
