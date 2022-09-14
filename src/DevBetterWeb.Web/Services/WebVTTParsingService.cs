using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DevBetterWeb.Web.Interfaces;

namespace DevBetterWeb.Web.Services;

public class WebVTTParsingService : IWebVTTParsingService
{
	private const string pattern = @"(?<start>\d{2,}:\d{2}:\d{2}\.\d{3})[ ]+-->[ ]+(?<end>\d{2,}:\d{2}:\d{2}\.\d{3})\s+(?<text>.+)";

	public string Parse(string vtt, string linkToVideo, int paragraphSize = 4)
	{
		var rg = new Regex(pattern);
		var matches = rg.Matches(vtt);

		var (sb, _) = matches
			.Select(m => RegExMatchToAnchorTag(m, linkToVideo))
			.Aggregate((Markup: new StringBuilder(), EndsWithPeriodCount: 0), (acc, x) => AddParagraphsAndBuildMarkup(paragraphSize, acc, x));

		return sb.ToString();
	}

	private static (string Markup, bool EndsWithPeriod) RegExMatchToAnchorTag(Match m, string linkToVideo)
	{
		var start = m.Groups["start"].Value;
		int seconds = (int)TimeSpan.Parse(start).TotalSeconds;
		var text = m.Groups["text"].Value.TrimEnd();
		return (Markup: $"<a href=\"{linkToVideo}/{seconds}\">{text}</a>", EndsWithPeriod: text.EndsWith("."));
	}

	private static (StringBuilder Markup, int EndsWithPeriodCount) AddParagraphsAndBuildMarkup(
		int paragraphSize, (StringBuilder Markup, int EndsWithPeriodCount) acc, (string Markup, bool EndsWithPeriod) x)
	{
		acc.Markup.AppendLine(x.Markup);
		acc.EndsWithPeriodCount = x.EndsWithPeriod ? acc.EndsWithPeriodCount + 1 : acc.EndsWithPeriodCount;
		if (acc.EndsWithPeriodCount == paragraphSize)
		{
			acc.Markup.AppendLine("<br><br>");
			acc.EndsWithPeriodCount = 0;
		}
		return acc;
	}
}