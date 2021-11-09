using System.Collections.Generic; 
using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class PublishToSocial
{
  [JsonPropertyName("options")]
  public List<string> Options { get; set; }

  [JsonPropertyName("publish_blockers")]
  public List<object> PublishBlockers { get; set; }

  [JsonPropertyName("publish_constraints")]
  public List<object> PublishConstraints { get; set; }

  [JsonPropertyName("uri")]
  public string Uri { get; set; }
}
