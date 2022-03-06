using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using DevBetterWeb.WpfUploader.ApiClients;
using DevBetterWeb.WpfUploader.Services;
using DevBetterWeb.WpfUploader.ViewModels;
using Video = DevBetterWeb.Vimeo.Models.Video;

namespace DevBetterWeb.WpfUploader;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
  private readonly GetVideosService _getVideosService;
  private readonly DeleteVideo _deleteVideo;

  public MainWindow()
  {
  }

  public MainWindow(GetVideosService getVideosService, DeleteVideo deleteVideo)
  {
    InitializeComponent();

    _getVideosService = getVideosService;
    _deleteVideo = deleteVideo;

    var iconUri = new Uri("pack://application:,,,/Resources/db-icon.png", UriKind.RelativeOrAbsolute);
    Icon = BitmapFrame.Create(iconUri);
  }

  private void BtnSelectFolder_Click(object sender, RoutedEventArgs e)
  {
    var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
    if (dialog.ShowDialog(this).GetValueOrDefault())
    {
      TxtFolderPath.Text = dialog.SelectedPath;
      var videos = GetVideosService.GetVideosFromFolder(dialog.SelectedPath);
      
      DataGridVideos.ItemsSource = GridDataVideoCollection.FromVimeoVideo(videos);
    }
  }

  private void BtnDeleteSelectedVideos_Click(object sender, RoutedEventArgs e)
  {
    var videos = GetSelectedVideos();
    var errorVideos = new List<string>();
    var successVideos = new List<string>();

    foreach (var video in videos)
    {
      var deleteResponse = _deleteVideo.ExecuteAsync(video.Id).Result;
      var responseCode = deleteResponse?.Code;

      if (responseCode != HttpStatusCode.OK)
      {
        errorVideos.Add(video.Id);
      }
      else
      {
        successVideos.Add(video.Id);
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

  private List<Video> GetSelectedVideos()
  {
    List<Video> videos = new List<Video>();

    IList items = DataGridVideos.SelectedItems;
    foreach (object item in items)
    {
      if (item is not Video selectedVideo) 
        continue;
      videos.Add(selectedVideo);
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
}

