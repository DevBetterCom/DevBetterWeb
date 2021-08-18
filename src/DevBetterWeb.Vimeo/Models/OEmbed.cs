using System.Text.Json.Serialization;

namespace DevBetterWeb.Vimeo.Models
{
  public class OEmbed
  {
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("provider_name")]
    public string ProviderName { get; set; }

    [JsonPropertyName("provider_url")]
    public string ProviderUrl { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("author_name")]
    public string AuthorName { get; set; }

    [JsonPropertyName("author_url")]
    public string AuthorUrl { get; set; }

    [JsonPropertyName("is_plus")]
    public string IsPlus { get; set; }

    [JsonPropertyName("account_type")]
    public string AccountType { get; set; }

    [JsonPropertyName("html")]
    public string Html { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("thumbnail_url")]
    public string ThumbnailUrl { get; set; }

    [JsonPropertyName("thumbnail_width")]
    public int ThumbnailWidth { get; set; }

    [JsonPropertyName("thumbnail_height")]
    public int ThumbnailHeight { get; set; }

    [JsonPropertyName("thumbnail_url_with_play_button")]
    public string ThumbnailUrlWithPlayButton { get; set; }

    [JsonPropertyName("upload_date")]
    public string UploadDate { get; set; }

    [JsonPropertyName("video_id")]
    public int VideoId { get; set; }

    [JsonPropertyName("uri")]
    public string Uri { get; set; }
  }


}
