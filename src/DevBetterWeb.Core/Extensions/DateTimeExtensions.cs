using System;

namespace DevBetterWeb.Core.Extensions;

public static class DateTimeExtensions
{
	public static long ConvertToUnixTimeSeconds(this DateTime value)
	{
		return new DateTimeOffset(value).ToUnixTimeSeconds();
	}
}
