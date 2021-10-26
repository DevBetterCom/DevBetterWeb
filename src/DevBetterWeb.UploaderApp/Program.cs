using System;
using System.Net.Http;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.Extensions.DependencyInjection;
using McMaster.Extensions.CommandLineUtils;
using System.ComponentModel.DataAnnotations;
using Serilog;
using Serilog.Events;

namespace DevBetterWeb.UploaderApp
{
  // TODO: Simplify using CommandLineUtils package
  // See: https://briancaos.wordpress.com/2020/02/12/command-line-parameters-in-net-core-console-applications/
  // TODO: Add a logger
  // TODO: Add a verbose flag to the command line that turns logger verbosity up/down

  public class AsyncProgram
  {
    // working from this example
    // https://github.com/natemcmaster/CommandLineUtils/blob/main/docs/samples/dependency-injection/generic-host/Program.cs

    private static IServiceProvider _serviceProvider;

    public static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<AsyncProgram>(args);

    [Argument(0, Description = "Folder with files to upload")]
    public string FolderPath { get; }

    [Required]
    [Option("-t|--token", Description = "Vimeo token")]
    public string Token { get; }

    [Required]
    [Option("-l|--link", Description = "devBetter API Link (ex. devbetter.com/)")]
    public string ApiLink { get; }

    [Required]
    [Option("-k|--key", Description = "devBetter API Key")]
    public string ApiKey { get; }

    [Option("-v|--verbose", Description = "Toggle logger verbosity: debug, trace, information, warning, error")]
    public string Verbose { get; } = "error";

    private async Task OnExecuteAsync()
    {
      // use properties here knowing they're initialized
      var folderPath = FolderPath ?? "world";

      var configInfo = new ConfigInfo(Token, ApiLink, ApiKey);
      _serviceProvider = SetupDi(configInfo);
      EnableLogger();

      Log.Debug("Logger Enabled");
      Log.Debug("DI Setup Done");

      var uploaderService = GetUploaderService();
      await uploaderService.SyncAsync(folderPath);

      Console.WriteLine("Done, press any key to close");
      Console.ReadKey();
    }

    private LogEventLevel? GetLoggerLevel(string level)
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

    private void EnableLogger()
    {
      var loggerConfiguration = new LoggerConfiguration();        

      var loggerLevel = GetLoggerLevel(Verbose);
      if (loggerLevel != null)
      {
        loggerConfiguration = loggerConfiguration.WriteTo.Console(restrictedToMinimumLevel: loggerLevel.Value);
      }

      Log.Logger = loggerConfiguration.CreateLogger();
    }

    private static ServiceProvider SetupDi(ConfigInfo configInfo)
    {
      var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton(configInfo)
            .AddScoped(sp => HttpClientBuilder())
            .AddScoped<HttpService>()
            .AddScoped<GetAllVideosService>()
            .AddScoped<GetAnimatedThumbnailService>()
            .AddScoped<GetStatusAnimatedThumbnailService>()
            .AddScoped<AddAnimatedThumbnailsToVideoService>()
            .AddScoped<AddDomainToVideoService>()
            .AddScoped<CompleteUploadByCompleteUriService>()
            .AddScoped<GetStreamingTicketService>()
            .AddScoped<UpdateVideoDetailsService>()
            .AddScoped<UploadVideoService>()
            .AddScoped<GetVideoService>()
            .AddScoped<UploaderService>();

      return services.BuildServiceProvider();
    }

    private static UploaderService GetUploaderService()
    {
      return _serviceProvider
        .GetService<UploaderService>();
    }

    private static HttpClient HttpClientBuilder()
    {
      var httpClient = new HttpClient
      {
        BaseAddress = new Uri(ServiceConstants.VIMEO_URI),
        Timeout = TimeSpan.FromHours(2)
      };
      httpClient.DefaultRequestHeaders.Add("Accept", ServiceConstants.VIMEO_HTTP_ACCEPT);

      return httpClient;
    }
  }
}
