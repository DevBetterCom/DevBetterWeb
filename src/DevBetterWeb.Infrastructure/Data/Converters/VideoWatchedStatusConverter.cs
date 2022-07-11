using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DevBetterWeb.Infrastructure.Data.Converters;

public class VideoWatchedStatusConverter : ValueConverter<VideoWatchedStatus, string>
{
	public VideoWatchedStatusConverter()
		: base(
			coreValue => ToString(coreValue),
			efValue => FromString(efValue))
	{
	}

	private static string ToString(VideoWatchedStatus type)
	{
		return type switch
		{
			VideoWatchedStatus.Unwatched => "U",
			VideoWatchedStatus.InProgress => "P",
			VideoWatchedStatus.Watched => "W",
			_ => throw new System.NotImplementedException(),
		};
	}

	private static VideoWatchedStatus FromString(string type)
	{
		return type.ToUpper() switch
		{
			"U" => VideoWatchedStatus.Unwatched,
			"P" => VideoWatchedStatus.InProgress,
			"W" => VideoWatchedStatus.Watched,
			_ => throw new System.NotImplementedException(),
		};
	}
}
