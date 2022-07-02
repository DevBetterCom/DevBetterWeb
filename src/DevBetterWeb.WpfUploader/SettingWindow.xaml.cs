using System;
using System.Windows;
using System.Windows.Media.Imaging;
using DevBetterWeb.WpfUploader.Managers;

namespace DevBetterWeb.WpfUploader;
/// <summary>
/// Interaction logic for SettingWindow.xaml
/// </summary>
public partial class SettingWindow : Window
{
  private readonly AppSettingsManager _appSettingsManager;

  public SettingWindow()
  {
    InitializeComponent();

    _appSettingsManager = new AppSettingsManager();
    TextBoxVimeoToken.Text = _appSettingsManager.GetAppSetting(AppConstants.TOKEN);
    TextBoxApiLink.Text = _appSettingsManager.GetAppSetting(AppConstants.API_LINK);
    TextBoxApiKey.Text = _appSettingsManager.GetAppSetting(AppConstants.API_KEY);

    var iconUri = new Uri("pack://application:,,,/Resources/db-icon.png", UriKind.RelativeOrAbsolute);
    Icon = BitmapFrame.Create(iconUri);
  }

  private void BtnSaveSettings_Click(object sender, RoutedEventArgs e)
  {
    _appSettingsManager.AddUpdateAppSettings(AppConstants.TOKEN, TextBoxVimeoToken.Text);
    _appSettingsManager.AddUpdateAppSettings(AppConstants.API_LINK, TextBoxApiLink.Text);
    _appSettingsManager.AddUpdateAppSettings(AppConstants.API_KEY, TextBoxApiKey.Text);

    App.ChangeConfigInfo(TextBoxVimeoToken.Text, TextBoxApiLink.Text, TextBoxApiKey.Text);

    Close();
  }
}
