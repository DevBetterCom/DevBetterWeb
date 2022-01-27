using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Services.VideoServices;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.Hosting.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Hosting;
using Serilog.Extensions.Logging;

namespace DevBetterWeb.UploaderApp;

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
  [Option("-l|--link", Description = "devBetter API Link (ex. https://devbetter.com/)")]
  public string ApiLink { get; }

  [Required]
  [Option("-k|--key", Description = "devBetter API Key")]
  public string ApiKey { get; }

  [Option("-u|--update-thumbnails", "Update Animated Thumbnails (The Vimeo ID is Required)", CommandOptionType.NoValue)]
  public bool IsUpdateThumbnails { get; }

  [Option("-i|--id", Description = "Vimeo ID")]
  public string VimeoId { get; }

  [Option("-v|--verbose", Description = "Toggle logger verbosity: debug, trace, info, warning, error")]
  public string Verbose { get; } = "error";

  private async Task OnExecuteAsync()
  {
    // ideally I'd like all of this setup of config, services, and logging to happen in the generic host builder above
    var configInfo = new ConfigInfo(Token, ApiLink, ApiKey);
    var logger = CreateLogger();
    _serviceProvider = SetupDi(configInfo, logger);

    logger.Debug("Logger Enabled"); // I'm never seeing these displayed in the console -Steve
    logger.Debug("DI Setup Done");

    // I'd like this to be the first line of OnExecuteAsync
    var uploaderService = GetUploaderService();
    if (IsUpdateThumbnails)
    {
      if (string.IsNullOrEmpty(VimeoId))
      {
        logger.Information("The Vimeo ID is Required");
      }
      else
      {
        await uploaderService.UpdateAnimatedThumbnailsAsync(VimeoId);
      }
    }
    else
    {
      await uploaderService.SyncAsync(FolderPath);
    }

    Console.WriteLine("Done, press any key to close");
    Console.ReadKey();
  }

  private Serilog.ILogger CreateLogger()
  {
    var loggerLevel = Verbose.ToLower().ToLogEventLevel().Value;

    var loggerConfiguration = new LoggerConfiguration()
      .MinimumLevel.Is(loggerLevel)
      .Enrich.FromLogContext()
      .WriteTo.Console();

    var logger = loggerConfiguration.CreateLogger();
    Log.Logger = logger;

    logger.Debug("Logger created with level {0}", loggerLevel);

    return logger;
  }

  private static ServiceProvider SetupDi(ConfigInfo configInfo, Serilog.ILogger logger)
  {
    var services = new ServiceCollection()
          .AddLogging()
          .AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory(logger, false))
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
          .AddScoped<UploaderService>()
          .AddScoped<ActiveTextTrackService>()
          .AddScoped<GetUploadLinkTextTrackService>()
          .AddScoped<UploadTextTrackFileService>()
          .AddScoped<UploadSubtitleToVideoService>()
          .AddScoped<GetAllTextTracksService>();

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
