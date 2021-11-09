using System; 
using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Subscribe
{
  [JsonPropertyName("drm")]
  public bool Drm { get; set; }

  [JsonPropertyName("expires_time")]
  public DateTime ExpiresTime { get; set; }

  [JsonPropertyName("purchase_time")]
  public DateTime PurchaseTime { get; set; }

  [JsonPropertyName("stream")]
  public string Stream { get; set; }
}
