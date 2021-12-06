using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using DevBetterWeb.Core.Interfaces;
using Discord;
using Newtonsoft.Json;

namespace DevBetterWeb.Infrastructure.DiscordWebooks;

/// <summary>
/// https://github.com/Hellsing/DiscordWebhook/blob/master/Webhook.cs
/// This works but I don't like that the service and the state are in the same type; 
/// services injected via DI should be stateless
/// </summary>
[JsonObject]
public abstract class BaseWebhook
{
  private readonly HttpClient _httpClient = new HttpClient();
  protected string _webhookUrl;
  private readonly IAppLogger<BaseWebhook> _logger;

  public BaseWebhook(string webhookUrl, IAppLogger<BaseWebhook> logger)
  {
    Guard.Against.NullOrEmpty(webhookUrl, nameof(webhookUrl));
    _webhookUrl = webhookUrl;
    _logger = logger;
  }

  public BaseWebhook(ulong id, string token, IAppLogger<BaseWebhook> logger) : this($"https://discordapp.com/api/webhooks/{id}/{token}", logger)
  {
  }

  [JsonProperty("content")]
  public string Content { get; set; } = "";
  [JsonProperty("username")]
  public string Username { get; set; } = "";
  [JsonProperty("avatar_url")]
  public string AvatarUrl { get; set; } = "";
  // ReSharper disable once InconsistentNaming
  [JsonProperty("tts")]
  public bool IsTTS { get; set; }
  [JsonProperty("embeds")]
  public List<Embed> Embeds { get; set; } = new List<Embed>();

  public virtual async Task<HttpResponseMessage> Send()
  {
    var content = new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");
    try
    {
      return await _httpClient.PostAsync(_webhookUrl, content);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error sending webhook", _webhookUrl, content);
      return new HttpResponseMessage();
    }
  }

  public Task<HttpResponseMessage> Send(string content, string username = "", string avatarUrl = "", bool isTTS = false, IEnumerable<Embed>? embeds = null)
  {
    Content = content;
    Username = username;
    AvatarUrl = avatarUrl;
    IsTTS = isTTS;
    Embeds.Clear();
    if (embeds != null)
    {
      Embeds.AddRange(embeds);
    }

    return Send();
  }
}
