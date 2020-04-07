using Ardalis.GuardClauses;
using Discord;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DevBetterWeb.Infrastructure.Services
{
    /// <summary>
    /// https://github.com/Hellsing/DiscordWebhook/blob/master/Webhook.cs
    /// This works but I don't like that the service and the state are in the same type; 
    /// services injected via DI should be stateless
    /// </summary>
    [JsonObject]
    public class Webhook
    {
        private readonly HttpClient _httpClient;
        private readonly string _webhookUrl;

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

        public Webhook(IOptions<DiscordWebhookUrls> optionsAccessor)
        {
            Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));
            Guard.Against.NullOrEmpty(optionsAccessor.Value.AdminUpdates, "AdminUpdates");

            _httpClient = new HttpClient();
            _webhookUrl = optionsAccessor.Value.AdminUpdates;
        }

        public Webhook(string webhookUrl)
        {
            _httpClient = new HttpClient();
            _webhookUrl = webhookUrl;
        }

        public Webhook(ulong id, string token) : this($"https://discordapp.com/api/webhooks/{id}/{token}")
        {
        }

        public async Task<HttpResponseMessage> Send()
        {
            var content = new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(_webhookUrl, content);
        }

        public async Task<HttpResponseMessage> Send(string content, string username = "", string avatarUrl = "", bool isTTS = false, IEnumerable<Embed>? embeds = null)
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

            return await Send();
        }
    }
}
