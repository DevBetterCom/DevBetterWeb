using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Block
{
  [JsonPropertyName("options")]
  public List<string> Options { get; set; }

  [JsonPropertyName("total")]
  public int Total { get; set; }

  [JsonPropertyName("uri")]
  public string Uri { get; set; }

  [JsonPropertyName("added")]
  public bool Added { get; set; }

  [JsonPropertyName("added_time")]
  public DateTime AddedTime { get; set; }
}
