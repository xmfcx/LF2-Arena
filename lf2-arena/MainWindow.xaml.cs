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
using MaterialDesignThemes.Wpf;

namespace lf2_arena
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private Lf2Handler _lf2Handler;

    public MainWindow()
    {
      InitializeComponent();
      RoomListModel roomListModel = new RoomListModel();
      DataContext = roomListModel;

      _lf2Handler = new Lf2Handler();
      _lf2Handler.OnRoomStateChanged += Lf2HandlerOnOnRoomStateChanged;
      _lf2Handler.ListenForLf2();
    }

    private void Lf2HandlerOnOnRoomStateChanged(bool roomState)
    {
      if (roomState)
      {
        Dispatcher.Invoke(() =>
        {
          BottomColorZone.Mode = ColorZoneMode.PrimaryLight;
          BottomText.Text = "LF2 is connected.";
        });
        return;
      }
      Dispatcher.Invoke(() =>
      {
        BottomColorZone.Mode = ColorZoneMode.Accent;
        BottomText.Text = "Waiting for LF2 to connect...";
      });
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