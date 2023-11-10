using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DevBetterWeb.Infrastructure.DiscordWebhooks;

public class Embed : IEmbedUrl
{
	[JsonPropertyName("title")]
	public string? Title { get; set; }
	[JsonPropertyName("type")]
	public string Type { get; set; } = "rich";
	[JsonPropertyName("description")]
	public string? Description { get; set; }
	public string? Url { get; set; }
	[JsonPropertyName("timestamp")]
	public DateTimeOffset? TimeStamp { get; set; }
	[JsonPropertyName("color")]
	public int Color { get; set; }
	[JsonPropertyName("footer")]
	public EmbedFooter? Footer { get; set; }
	[JsonPropertyName("image")]
	public EmbedImage? Image { get; set; }
	[JsonPropertyName("thumbnail")]
	public EmbedThumbnail? Thumbnail { get; set; }
	[JsonPropertyName("video")]
	public EmbedVideo? Video { get; set; }
	[JsonPropertyName("provider")]
	public EmbedProvider? Provider { get; set; }
	[JsonPropertyName("author")]
	public EmbedAuthor? Author { get; set; }
	[JsonPropertyName("fields")]
	public List<EmbedField> Fields { get; set; } = new List<EmbedField>();
}

public class EmbedFooter : IEmbedIconUrl, IEmbedIconProxyUrl
{
	[JsonPropertyName("text")]
	public string? Text { get; set; }
	public string? IconUrl { get; set; }
	public string? ProxyIconUrl { get; set; }
}

public class EmbedImage : EmbedProxyUrl, IEmbedDimension
{
	public int Height { get; set; }
	public int Width { get; set; }
}

public class EmbedThumbnail : EmbedProxyUrl, IEmbedDimension
{
	public int Height { get; set; }
	public int Width { get; set; }
}

public class EmbedVideo : EmbedUrl, IEmbedDimension
{
	public int Height { get; set; }
	public int Width { get; set; }
}

public class EmbedProvider : EmbedUrl
{
	[JsonPropertyName("name")]
	public string? Name { get; set; }
}

public class EmbedAuthor : EmbedUrl, IEmbedIconUrl, IEmbedIconProxyUrl
{
	[JsonPropertyName("name")]
	public string? Name { get; set; }
	public string? IconUrl { get; set; }
	public string? ProxyIconUrl { get; set; }
}

public class EmbedField
{
	[JsonPropertyName("name")]
	public string? Name { get; set; }
	[JsonPropertyName("value")]
	public string? Value { get; set; }
	[JsonPropertyName("inline")]
	public bool Inline { get; set; }
}

public abstract class EmbedUrl : IEmbedUrl
{
	public string? Url { get; set; }
}

public abstract class EmbedProxyUrl : EmbedUrl, IEmbedProxyUrl
{
	public string? ProxyUrl { get; set; }
}

public interface IEmbedUrl
{
	[JsonPropertyName("url")]
	string? Url { get; set; }
}

public interface IEmbedProxyUrl
{
	[JsonPropertyName("proxy_url")]
	string? ProxyUrl { get; set; }
}

public interface IEmbedIconUrl
{
	[JsonPropertyName("icon_url")]
	string? IconUrl { get; set; }
}

public interface IEmbedIconProxyUrl
{
	[JsonPropertyName("proxy_icon_url")]
	string? ProxyIconUrl { get; set; }
}

public interface IEmbedDimension
{
	[JsonPropertyName("height")]
	int Height { get; set; }
	[JsonPropertyName("width")]
	int Width { get; set; }
}

internal class WebHookRequest
{
	[JsonPropertyName("content")]
	public string Content { get; set; }
	[JsonPropertyName("username")]
	public string? Username { get; set; }
	[JsonPropertyName("avatar_url")]
	public string? AvatarUrl { get; set; }
	// ReSharper disable once InconsistentNaming
	[JsonPropertyName("tts")]
	public bool IsTTS { get; set; }
	[JsonPropertyName("embeds")]
	public List<Embed>? Embeds { get; set; } = new List<Embed>();

	public WebHookRequest(string content, string? username = null, string? avatarUrl = null, bool isTTS = false, IEnumerable<Embed>? embeds = null)
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
	}
}
