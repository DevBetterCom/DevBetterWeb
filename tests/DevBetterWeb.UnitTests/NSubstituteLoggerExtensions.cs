using System;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace DevBetterWeb.UnitTests;

/// <summary>
/// Extension methods to help verify calls to <see cref="ILogger"/>'s various extension methods
/// that cannot be mocked/verified through normal means.
/// </summary>
internal static class NSubstituteLoggerExtensions
{
	/// <summary>
	/// Use when you want to verify that <see cref="LoggerExtensions.LogInformation(Microsoft.Extensions.Logging.ILogger,Microsoft.Extensions.Logging.EventId,System.Exception?,string?,object?[])"/>
	/// was called <paramref name="numberOfCalls"/> time(s) and with what message you expect.
	/// </summary>
	public static void ReceivedLogInformation(this ILogger mockedLogger, int numberOfCalls, string expectedMessage)
	{
		mockedLogger.Received(numberOfCalls).Log(LogLevel.Information, Arg.Any<EventId>(), expectedMessage);
	}

	/// <summary>
	/// Use when you want to verify that <see cref="LoggerExtensions.LogError(Microsoft.Extensions.Logging.ILogger,Microsoft.Extensions.Logging.EventId,System.Exception?,string?,object?[])"/>
	/// was called <paramref name="numberOfCalls"/> time(s) and with what excpetion and message you expect.
	/// </summary>
	public static void ReceivedLogError(this ILogger mockedLogger, int numberOfCalls, Exception expectedException, string expectedMessage)
	{
		mockedLogger.Received(numberOfCalls).Log(LogLevel.Error, Arg.Any<EventId>(), expectedException, expectedMessage);
	}
}
