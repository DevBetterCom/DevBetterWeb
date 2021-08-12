using System;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Vimeo.Extensions
{
  public static class LoggerExtensions
  {
    public static void LogError(this ILogger logger, Exception exception)
    {
      logger.LogError($"Error occurred {exception.Message} {exception.StackTrace} {exception.InnerException} {exception.Source}");
    }
  }
}
