using System.Windows;
using DevBetterWeb.Vimeo.Extensions;
using DevBetterWeb.WpfUploader.ApiClients;
using DevBetterWeb.WpfUploader.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DevBetterWeb.WpfUploader;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
  private readonly ServiceProvider _serviceProvider;
  public App()
  {
    ServiceCollection services = new ServiceCollection();
    ConfigureServices(services);
    _serviceProvider = services.BuildServiceProvider();
  }
  private void ConfigureServices(ServiceCollection services)
  {
    services.AddVimeoServices();
    services.AddScoped<GetVideosService>();
    services.AddScoped<DeleteVideo>();
    services.AddSingleton<MainWindow>();
  }
  private void OnStartup(object sender, StartupEventArgs e)
  {
    var mainWindow = _serviceProvider.GetService<MainWindow>();
    mainWindow?.Show();
  }
}

