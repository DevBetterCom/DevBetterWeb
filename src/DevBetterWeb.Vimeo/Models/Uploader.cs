using System.Collections.Generic; 
using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Uploader
{
  [JsonPropertyName("pictures")]
  public Pictures Pictures { get; set; }
}
