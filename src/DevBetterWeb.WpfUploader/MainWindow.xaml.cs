using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DevBetterWeb.WpfUploader;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
  public MainWindow()
  {
    InitializeComponent();

    var iconUri = new Uri("pack://application:,,,/Resources/db-icon.png", UriKind.RelativeOrAbsolute);
    Icon = BitmapFrame.Create(iconUri);
  }

  private void BtnSelectFolder_OnClick(object sender, RoutedEventArgs e)
  {
    var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
    if (dialog.ShowDialog(this).GetValueOrDefault())
    {
      TxtFolderPath.Text = dialog.SelectedPath;
    }
  }
}

