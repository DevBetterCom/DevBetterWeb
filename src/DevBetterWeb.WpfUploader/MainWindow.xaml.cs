using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DevBetterWeb.WpfUploader.Services;
using DevBetterWeb.WpfUploader.ViewModels;
using Video = DevBetterWeb.Vimeo.Models.Video;

namespace DevBetterWeb.WpfUploader;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
  private readonly UploaderService _uploaderService;

  public MainWindow(UploaderService uploaderService)
  {
    InitializeComponent();
    _uploaderService = uploaderService;

    var iconUri = new Uri("pack://application:,,,/Resources/db-icon.png", UriKind.RelativeOrAbsolute);
    Icon = BitmapFrame.Create(iconUri);


  }

  private List<Video> GetSelectedVideos()
  {
    List<Video> videos = new List<Video>();

    IList items = DataGridVideos.SelectedItems;
    foreach (object item in items)
    {
      videos.Add(((GridDataVideo)item).ToVimeoVideo());
    }

    return videos;
  }

  private void ShowInfoMessage(string caption, string messageBoxText)
  {
    const MessageBoxButton button = MessageBoxButton.OK;
    const MessageBoxImage icon = MessageBoxImage.Information;

    var result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
  }

  private void ShowErrorMessage(string caption, string messageBoxText)
  {
    const MessageBoxButton button = MessageBoxButton.OK;
    const MessageBoxImage icon = MessageBoxImage.Error;

    var result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
  }

  private void TxtFolderPath_TextChanged(object sender, TextChangedEventArgs e)
  {
    TextBox? box = sender as TextBox;

    if (string.IsNullOrEmpty(box?.Text) || box.Text.Length == 0)
    {
      BtnSyncVideos.IsEnabled = false;
    }
    else
    {
      BtnSyncVideos.IsEnabled = true;
    }
  }

  private void TxtVideoId_TextChanged(object sender, TextChangedEventArgs e)
  {
    TextBox? box = sender as TextBox;

    if (string.IsNullOrEmpty(box?.Text) || box.Text.Length == 0)
    {
      BtnUpdateVideoThumb.IsEnabled = false;
      BtnDeleteVideo.IsEnabled = false;
    }
    else
    {
      BtnUpdateVideoThumb.IsEnabled = true;
      BtnDeleteVideo.IsEnabled = true;
    }
  }

  private void DataGridVideos_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    var selectedVideos = GetSelectedVideos();
    if (selectedVideos.Count >= 0)
    {
      BtnUploadSelectedVideos.IsEnabled = true;
      BtnUpdateThumbs.IsEnabled = true;
      BtnDeleteVideos.IsEnabled = true;
    }
    else
    {
      BtnUploadSelectedVideos.IsEnabled = false;
      BtnUpdateThumbs.IsEnabled = false;
      BtnDeleteVideos.IsEnabled = false;
    }
  }

  private async void BtnUploadSelectedVideos_Click(object sender, RoutedEventArgs e)
  {
    var selectedVideos = GetSelectedVideos();
    await _uploaderService.UploadSelectedVideosAsync(TxtFolderPath.Text, selectedVideos);
  }

  private async void BtnUpdateThumbs_Click(object sender, RoutedEventArgs e)
  {
    var selectedVideos = GetSelectedVideos();
    await _uploaderService.UpdateVideosAnimatedThumbnailsAsync(selectedVideos);
  }

  private async void BtnSelectFolder_Click(object sender, RoutedEventArgs e)
  {
    var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
    if (dialog.ShowDialog(this).GetValueOrDefault())
    {
      TxtFolderPath.Text = dialog.SelectedPath;
      var videos = await _uploaderService.LoadVideosAsync(TxtFolderPath.Text);

      DataGridVideos.ItemsSource = GridDataVideoCollection.FromVimeoVideo(videos);
    }
  }

  private async void BtnDeleteSelectedVideos_Click(object sender, RoutedEventArgs e)
  {
    var videos = GetSelectedVideos();
    var errorVideos = new List<string>();
    var successVideos = new List<string>();

    foreach (var video in videos)
    {
      var deleteResponse = await _uploaderService.DeleteVimeoVideoAsync(video.Id);

      if (deleteResponse)
      {
        successVideos.Add(video.Id);
      }
      else
      {
        errorVideos.Add(video.Id);
      }
    }

    if (errorVideos.Count > 0)
    {
      ShowErrorMessage("Video Delete", $"Error: {string.Join(",", errorVideos.ToArray())}");
    }

    if (successVideos.Count > 0)
    {
      ShowInfoMessage("Video Delete", $"Success: {string.Join(",", successVideos.ToArray())}");
    }
  }

  private async void BtnDeleteVideo_Click(object sender, RoutedEventArgs e)
  {
    var videoId = TxtVideoId.Text;

    var deleteResponse = await _uploaderService.DeleteVimeoVideoAsync(videoId);

    if (deleteResponse)
    {
      ShowInfoMessage("Video Delete", $"Success: {videoId}");
    }
    else
    {
      ShowErrorMessage("Video Delete", $"Error: {videoId}");
    }
  }

  private void BtnShowSettings_Click(object sender, RoutedEventArgs e)
  {
    var settingWindow = new SettingWindow();
    settingWindow.Show();
  }

  private async void BtnSync_Click(object sender, RoutedEventArgs e)
  {
    await _uploaderService.SyncAsync(TxtFolderPath.Text);
  }

  private async void BtnUpdateVideoThumb_Click(object sender, RoutedEventArgs e)
  {
    var videoId = TxtVideoId.Text;
    await _uploaderService.UpdateAnimatedThumbnailsAsync(videoId);
  }
}

