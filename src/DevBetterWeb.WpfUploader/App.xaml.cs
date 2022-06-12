using System;
using System.Net.Http;
using System.Windows;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.WpfUploader.ApiClients;
using DevBetterWeb.WpfUploader.Managers;
using DevBetterWeb.WpfUploader.Models;
using DevBetterWeb.WpfUploader.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace DevBetterWeb.WpfUploader;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
  private static ServiceProvider _serviceProvider;
  private readonly AppSettingsManager _appSettingsManager;
  public App()
  {
    _appSettingsManager = new AppSettingsManager();

    var services = new ServiceCollection();
    ConfigureServices(services);
    _serviceProvider = services.BuildServiceProvider();
  }

  public static void ChangeConfigInfo(string token, string apiLink, string apiKey)
  {
    var configInfo = _serviceProvider.GetRequiredService<ConfigInfo>();
    configInfo.Update(token, apiLink, apiKey);
  }

  private void ConfigureServices(ServiceCollection services)
  {
    services.AddSingleton<MainWindow>();
    services.AddVimeoServices();
    
    var logger = CreateLogger();
    services.AddLogging();
    services.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory(logger, false));

    var configInfo = new ConfigInfo(_appSettingsManager.GetAppSetting(AppConstants.TOKEN), _appSettingsManager.GetAppSetting(AppConstants.API_LINK), _appSettingsManager.GetAppSetting(AppConstants.API_KEY));
    services.AddSingleton(configInfo);
    
    services.AddScoped<UploaderService>();
    services.AddScoped<GetVideosService>();
    services.AddScoped<DeleteVideo>();
  }

  
  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);
    var mainWindow = _serviceProvider.GetService<MainWindow>();
    mainWindow?.Show();
  }

  private Serilog.ILogger CreateLogger()
  {
    var loggerLevel = LogEventLevel.Information;

    var loggerConfiguration = new LoggerConfiguration()
      .MinimumLevel.Is(loggerLevel)
      .Enrich.FromLogContext()
      .WriteTo.Console();

    var logger = loggerConfiguration.CreateLogger();
    Log.Logger = logger;

    logger.Debug("Logger created with level {0}", loggerLevel);

    return logger;
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

