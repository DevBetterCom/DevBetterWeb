using System.Collections.Generic;
using DevBetterWeb.Web.Models;
using NimblePros.Vimeo.Models;

namespace DevBetterWeb.Web.Pages.Admin.Videos;

public class OEmbedViewModel
{
  public string? Name { get; set; }
  public string? Password { get; set; }
  public string? Type { get; set; }
  public string? Version { get; set; }
  public string? ProviderName { get; set; }
  public string? ProviderUrl { get; set; }
  public string? Title { get; set; }
  public string? AuthorName { get; set; }
  public string? AuthorUrl { get; set; }
  public string? IsPlus { get; set; }
  public string? AccountType { get; set; }
  public string? Html { get; set; }
  public int Width { get; set; }
  public int Height { get; set; }
  public int Duration { get; set; }
  public string? Description { get; set; }
  public string? DescriptionMd { get; set; } = string.Empty;
  public string? ThumbnailUrl { get; set; }
  public int ThumbnailWidth { get; set; }
  public int ThumbnailHeight { get; set; }
  public string? ThumbnailUrlWithPlayButton { get; set; }
  public string? UploadDate { get; set; }
  public int VideoId { get; set; }
  public string? Uri { get; set; }
  public string? CustomEmbedLink { get; set; } = string.Empty;
  public bool IsMemberFavorite { get; set; }
  public bool IsMemberWatched { get; set; }
  public int MemberFavoritesCount { get; set; }
	public List<VideoCommentDto> Comments { get; set; } = new();

  public OEmbedViewModel(OEmbed oEmbed)
  {
    Type = oEmbed.Type;
    Version = oEmbed.Version;
    ProviderName = oEmbed.ProviderName;
    ProviderUrl = oEmbed.ProviderUrl;
    Title = oEmbed.Title;
    AuthorName = oEmbed.AuthorName;
    AuthorUrl = oEmbed.AuthorUrl;
    IsPlus = oEmbed.IsPlus;
    AccountType = oEmbed.AccountType;
    Html = oEmbed.Html;
    Duration = oEmbed.Duration;
    Description = oEmbed.Description;
    ThumbnailUrl = oEmbed.ThumbnailUrl;
    ThumbnailWidth = oEmbed.ThumbnailWidth;
    ThumbnailHeight = oEmbed.ThumbnailHeight;
    ThumbnailUrlWithPlayButton = oEmbed.ThumbnailUrlWithPlayButton;
    UploadDate = oEmbed.UploadDate;
    VideoId = oEmbed.VideoId;
    Uri = oEmbed.Uri;
  }

	public OEmbedViewModel AddStartTime(string? startTime)
  {
    if (string.IsNullOrEmpty(startTime))
    {
      return this;
    }
    CustomEmbedLink += $"autoplay=1#t={startTime}&";

    return this;
  }

  public OEmbedViewModel BuildHtml(string link)
  {
    Html = Html?.Replace("iframe", "iframe id='videoIframe'");

    if (string.IsNullOrEmpty(link) || string.IsNullOrEmpty(CustomEmbedLink))
    {
      return this;
    }
    var parts = link.Split("/");
    if (parts.Length == 0)
    {
      return this;
    }
    var vedioId = parts[parts.Length - 1];

    CustomEmbedLink.Substring(CustomEmbedLink.Length - 1);
    Html = Html?.Replace($"{vedioId}?", $"{vedioId}?{CustomEmbedLink}");

    return this;
  }
}
