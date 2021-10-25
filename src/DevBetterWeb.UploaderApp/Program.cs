using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Ardalis.ApiCaller;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.Extensions.DependencyInjection;
using McMaster.Extensions.CommandLineUtils;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

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
      var subject = FolderPath ?? "world";

      // This pause here is just for indication that some awaitable operation could happens here.
      await Task.Delay(5000);


      Console.WriteLine($"Hello {subject}!");
    }
  }



  class Program2
  {
    private static IServiceProvider _serviceProvider;
    static async Task Main2(string[] args)
    {
      var argsList = args.ToList();
      if (argsList.Count == 0 || argsList.All(x => x.ToLower() != "-d") || argsList.All(x => x.ToLower() != "-t") || argsList.All(x => x.ToLower() != "-a") || argsList.All(x => x.ToLower() != "-akey"))
      {
        Console.WriteLine("Please use -d [destination folder] -t [Vimeo token] -a [api link] -akey [api key]");
        return;
      }

      var folderToUpload = GetArgument(argsList, "-d");
      if (string.IsNullOrEmpty(folderToUpload))
      {
        Console.WriteLine("Please use -d [destination folder]");
        return;
      }

      var token = GetArgument(argsList, "-t");
      if (string.IsNullOrEmpty(token))
      {
        Console.WriteLine("Please use -t [Vimeo token]");
        return;
      }

      var apiLink = GetArgument(argsList, "-a");
      if (string.IsNullOrEmpty(apiLink))
      {
        Console.WriteLine("Please use -a [api link]");
        return;
      }

      var apiKey = GetArgument(argsList, "-akey");
      if (string.IsNullOrEmpty(apiKey))
      {
        Console.WriteLine("Please use -akey [api key]");
        return;
      }

      var configInfo = new ConfigInfo(token, apiLink, apiKey);
      _serviceProvider = SetupDi(configInfo);

      var uploaderService = GetUploaderService();
      await uploaderService.SyncAsync(folderToUpload);

      Console.WriteLine("Done, press any key to close");
      Console.ReadKey();
    }

    private static string GetArgument(List<string> argsList, string argValue)
    {
      var index = argsList.FindIndex(x => x.ToLower() == argValue) + 1;
      if (index <= 0)
      {
        return null;
      }

      return argsList[index];
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
