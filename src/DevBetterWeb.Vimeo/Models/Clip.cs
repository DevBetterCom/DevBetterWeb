using System; 
using System.Collections.Generic; 
using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Clip
{
  [JsonPropertyName("categories")]
  public List<Category> Categories { get; set; }

  [JsonPropertyName("content_rating")]
  public List<string> ContentRating { get; set; }

  [JsonPropertyName("context")]
  public Context Context { get; set; }

  [JsonPropertyName("created_time")]
  public DateTime CreatedTime { get; set; }

  [JsonPropertyName("description")]
  public string Description { get; set; }

  [JsonPropertyName("duration")]
  public long Duration { get; set; }

  [JsonPropertyName("edit_session")]
  public List<EditSession> EditSession { get; set; }

  [JsonPropertyName("embed")]
  public List<Embed> Embed { get; set; }

  [JsonPropertyName("files")]
  public Files Files { get; set; }

  [JsonPropertyName("has_audio")]
  public string HasAudio { get; set; }

  [JsonPropertyName("height")]
  public int Height { get; set; }

  [JsonPropertyName("is_playable")]
  public string IsPlayable { get; set; }

  [JsonPropertyName("language")]
  public string Language { get; set; }

  [JsonPropertyName("last_user_action_event_date")]
  public DateTime LastUserActionEventDate { get; set; }

  [JsonPropertyName("license")]
  public string License { get; set; }

  [JsonPropertyName("link")]
  public string Link { get; set; }

  [JsonPropertyName("manage_link")]
  public string ManageLink { get; set; }

  [JsonPropertyName("metadata")]
  public Metadata Metadata { get; set; }

  [JsonPropertyName("modified_time")]
  public DateTime ModifiedTime { get; set; }

  [JsonPropertyName("name")]
  public string Name { get; set; }

  [JsonPropertyName("parent_folder")]
  public List<ParentFolder> ParentFolder { get; set; }

  [JsonPropertyName("password")]
  public string Password { get; set; }

  [JsonPropertyName("pictures")]
  public Pictures Pictures { get; set; }

  [JsonPropertyName("privacy")]
  public Privacy Privacy { get; set; }

  [JsonPropertyName("release_time")]
  public DateTime ReleaseTime { get; set; }

  [JsonPropertyName("resource_key")]
  public string ResourceKey { get; set; }

  [JsonPropertyName("spatial")]
  public Spatial Spatial { get; set; }

  [JsonPropertyName("stats")]
  public Stats Stats { get; set; }

  [JsonPropertyName("status")]
  public string Status { get; set; }

  [JsonPropertyName("tags")]
  public List<Tag> Tags { get; set; }

  [JsonPropertyName("transcode")]
  public Transcode Transcode { get; set; }

  [JsonPropertyName("type")]
  public string Type { get; set; }

  [JsonPropertyName("upload")]
  public Upload Upload { get; set; }

  [JsonPropertyName("uploader")]
  public Uploader Uploader { get; set; }

  [JsonPropertyName("uri")]
  public string Uri { get; set; }

  [JsonPropertyName("user")]
  public User User { get; set; }

  [JsonPropertyName("vod")]
  public List<object> Vod { get; set; }

  [JsonPropertyName("width")]
  public int Width { get; set; }
}
