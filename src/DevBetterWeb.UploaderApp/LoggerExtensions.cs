using Serilog.Events;

namespace DevBetterWeb.UploaderApp;

public static class LoggerExtensions
{
  public static LogEventLevel? ToLogEventLevel(this string level)
  {
    if (string.IsNullOrEmpty(level))
    {
      return null;
    }
    else if (level == "error")
    {
      return LogEventLevel.Error;
    }
    else if (level == "debug")
    {
      return LogEventLevel.Debug;
    }
    else if (level == "trace")
    {
      return LogEventLevel.Verbose;
    }
    else if (level == "info")
    {
      return LogEventLevel.Information;
    }
    else if (level == "warning")
    {
      return LogEventLevel.Warning;
    }
    else
    {
      return LogEventLevel.Verbose;
    }
  }
}
