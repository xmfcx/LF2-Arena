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

namespace lf2_arena
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      RoomListModel roomListModel = new RoomListModel();
      DataContext = roomListModel;
    }

    private void ButtonHost_OnClick(object sender, RoutedEventArgs e)
    {
      Transitioner.SelectedIndex = 1;
    }

    private void ButtonJoin_OnClick(object sender, RoutedEventArgs e)
    {
      Transitioner.SelectedIndex = 2;
    }

    private void ButtonExitRoom_OnClick(object sender, RoutedEventArgs e)
    {
      Transitioner.SelectedIndex = 0;
    }

    private void ButtonCancelGame_OnClick(object sender, RoutedEventArgs e)
    {
      Transitioner.SelectedIndex = 0;
    }

    private void ButtonPlay_OnClick(object sender, RoutedEventArgs e)
    {
      if (ButtonPlay.IsChecked == null)
        return;

      if ((bool) ButtonPlay.IsChecked)
      {
        Helper.LaunchLf2(3, @"E:\Games\LF2");
        return;
      }
      Helper.KillLf2();
    }
  }
}