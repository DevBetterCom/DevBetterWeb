using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using MediaInfo;

namespace DevBetterWeb.Vimeo.Models;

public class Video
{
  [JsonIgnore]
  public string Id
  {
    get
    {
      if (string.IsNullOrEmpty(Link))
      {
        return null;
      }
      var parts = Link.Split("/");
      if (parts.Length == 0)
      {
        return null;
      }
      var vedioId = parts[parts.Length - 1];

      return vedioId;
    }
  }

  [JsonIgnore]
  public byte[] Data { get; set; }

  [JsonIgnore]
  public string Subtitle { get; set; }

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
  public int Duration { get; set; }

  [JsonPropertyName("edit_session")]
  public List<EditSession> EditSession { get; set; }

  [JsonPropertyName("embed")]
  public Embed Embed { get; set; }

  //[JsonPropertyName("files")]
  //public Files Files { get; set; }

  [JsonPropertyName("has_audio")]
  public bool HasAudio { get; set; }

  [JsonPropertyName("height")]
  public int Height { get; set; }

  [JsonPropertyName("is_playable")]
  public bool IsPlayable { get; set; }

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
  public Privacy Privacy { get; set; } = new Privacy();

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

  public Video SetName(string name)
  {
    Name = name;

    return this;
  }

  public Video SetDescription(string description)
  {
    Description = description;

    return this;
  }

  public Video SetDuration(int duration)
  {
    Duration = duration;

    return this;
  }

  public Video SetSubtitle(string subtitle)
  {
    Subtitle = subtitle;

    return this;
  }

  public DateTime? GetEncodedDate(byte[] fileData)
  {
    string result = Path.GetTempPath();
    var fileNameTemp = Path.Combine(result, "temp.mp4");
    if (!ByteArrayToFile(fileNameTemp, fileData))
    {
      return null;
    }
    var mediaInfo = new MediaInfoWrapper(fileNameTemp);

    return mediaInfo.Tags.EncodedDate;
  }

  public Video SetReleaseTime(DateTime? releaseTime, bool isNow = false)
  {
    if (isNow)
    {
      ReleaseTime = DateTime.UtcNow;
      return this;
    }

    if (releaseTime != null)
    {
      ReleaseTime = releaseTime.Value;
    }

    return this;
  }

  public Video SetCreatedTime(DateTime? createdTime, bool isNow = false)
  {
    if (isNow)
    {
      ReleaseTime = DateTime.UtcNow;
      return this;
    }

    if (createdTime != null)
    {
      CreatedTime = createdTime.Value;
    }

    return this;
  }

  public Video SetModifiedTime(DateTime? modifiedTime, bool isNow = false)
  {
    if (isNow)
    {
      ReleaseTime = DateTime.UtcNow;
      return this;
    }

    if (modifiedTime != null)
    {
      ModifiedTime = modifiedTime.Value;
    }

    return this;
  }

  public Video SetEmbedProtecedPrivacy()
  {
    Privacy.View = PrivacyViewType.DISABLE_TYPE;
    Privacy.Embed = AccessType.WHITELIST_TYPE;
    Privacy.Download = false;

    return this;
  }

  public Video SetEmbed(bool isShare = true, bool isHd = true, bool isFullScreen = true, bool isLike = true, bool isWatchLater = true, bool isScaling = true)
  {
    if (Embed == null)
    {
      Embed = new Embed();
      Embed.Playbar = true;
      Embed.Speed = true;
      Embed.Volume = true;
    }

    if (Embed.Buttons == null)
    {
      Embed.Buttons = new Buttons();
    }

    Embed.Buttons.Share = isShare;
    Embed.Buttons.Fullscreen = isFullScreen;
    Embed.Buttons.Hd = isHd;
    Embed.Buttons.Watchlater = isWatchLater;
    Embed.Buttons.Scaling = isScaling;
    Embed.Buttons.Like = isLike;

    return this;
  }

  private bool ByteArrayToFile(string fileName, byte[] byteArray)
  {
    try
    {
      using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
      {
        fs.Write(byteArray, 0, byteArray.Length);
        return true;
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine("Exception caught in process: {0}", ex);
      return false;
    }
  }
}
