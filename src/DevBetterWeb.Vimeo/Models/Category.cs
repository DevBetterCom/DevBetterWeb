using System.Text.Json.Serialization;
using System.Collections.Generic;
using System;
namespace DevBetterWeb.Vimeo.Models
{

  public class Category
  {
    [JsonPropertyName("icon")]
    public Icon Icon { get; set; }

    [JsonPropertyName("is_deprecated")]
    public bool IsDeprecated { get; set; }

    [JsonPropertyName("last_video_featured_time")]
    public DateTime LastVideoFeaturedTime { get; set; }

    [JsonPropertyName("link")]
    public string Link { get; set; }

    [JsonPropertyName("metadata")]
    public Metadata Metadata { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("parent")]
    public Parent Parent { get; set; }

    [JsonPropertyName("pictures")]
    public Pictures Pictures { get; set; }

    [JsonPropertyName("resource_key")]
    public string ResourceKey { get; set; }

    [JsonPropertyName("subcategories")]
    public List<Subcategory> Subcategories { get; set; }

    [JsonPropertyName("top_level")]
    public bool TopLevel { get; set; }

    [JsonPropertyName("uri")]
    public string Uri { get; set; }

    [JsonPropertyName("options")]
    public List<string> Options { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }
  }

}
