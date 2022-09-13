using System.Text.RegularExpressions;
using System.Text;
using System;

namespace DevBetterWeb.Web.Services;

public  class WebVTTParsingService
{
	public string Parse(string vtt, string videoId)
	{
		string pattern = @"(?<start>\d{2,}:\d{2}:\d{2}\.\d{3})[ ]+-->[ ]+(?<end>\d{2,}:\d{2}:\d{2}\.\d{3})\s+(?<text>.+)";
		var rg = new Regex(pattern);
		var matches = rg.Matches(vtt);

		var sb = new StringBuilder();
		foreach (Match match in matches)
		{
			var start = match.Groups["start"].Value;
			var text = match.Groups["text"].Value.TrimEnd();

			int seconds = (int)TimeSpan.Parse(start).TotalSeconds;

			sb.AppendLine($"<a href=\"https://devbetter.com/Videos/Details/{videoId}/{seconds}\">{text}</a>");
		}

		return sb.ToString();
	}
}