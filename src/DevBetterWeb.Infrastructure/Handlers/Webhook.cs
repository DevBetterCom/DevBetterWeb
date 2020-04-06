using Discord;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Handlers
{
    /// <summary>
    /// https://github.com/Hellsing/DiscordWebhook/blob/master/Webhook.cs
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
