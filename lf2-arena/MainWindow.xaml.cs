using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;

namespace lf2_arena
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private SettingsHandler _settingsHandler;

    public MainWindow()
    {
      Helper.KillLf2();
      InitializeComponent();
      _settingsHandler = new SettingsHandler();
      _settingsHandler.OnKeyChangeEventOccured += KeyboardHookHandler.SetKeys;
      _settingsHandler.ForceKeyChangeEvent();
      var config = _settingsHandler.Config;
      TextBoxNamePlayer.Text = config.NamePlayer;
      TextBoxPathLf2.Text = config.PathLf2;
      TextBoxUp.Text = config.KeyUp.ToString();
      TextBoxDown.Text = config.KeyDown.ToString();
      TextBoxLeft.Text = config.KeyLeft.ToString();
      TextBoxRight.Text = config.KeyRight.ToString();
      TextBoxAttack.Text = config.KeyAttack.ToString();
      TextBoxJump.Text = config.KeyJump.ToString();
      TextBoxDefend.Text = config.KeyDefend.ToString();

      RoomListModel roomListModel = new RoomListModel();
      DataContext = roomListModel;


      var lf2Handler = new Lf2Handler();
      lf2Handler.OnRoomStateChanged += Lf2HandlerOnRoomStateChanged;
      KeyboardHookHandler.OnKeyEventOccured += lf2Handler.SetKeyString;
      lf2Handler.ListenForLf2();
    }

    private void Lf2HandlerOnRoomStateChanged(bool roomState)
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

    private void TextBoxUp_OnKeyUp(object sender, KeyEventArgs e)
    {
      TextBox textbox = (TextBox) sender;
      textbox.Text = e.Key.ToString();
      _settingsHandler.Config.KeyUp = e.Key;
      _settingsHandler.Save();
    }

    private void TextBoxDown_OnKeyUp(object sender, KeyEventArgs e)
    {
      TextBox textbox = (TextBox) sender;
      textbox.Text = e.Key.ToString();
      _settingsHandler.Config.KeyDown = e.Key;
      _settingsHandler.Save();
    }

    private void TextBoxLeft_OnKeyUp(object sender, KeyEventArgs e)
    {
      TextBox textbox = (TextBox) sender;
      textbox.Text = e.Key.ToString();
      _settingsHandler.Config.KeyLeft = e.Key;
      _settingsHandler.Save();
    }

    private void TextBoxRight_OnKeyUp(object sender, KeyEventArgs e)
    {
      TextBox textbox = (TextBox) sender;
      textbox.Text = e.Key.ToString();
      _settingsHandler.Config.KeyRight = e.Key;
      _settingsHandler.Save();
    }

    private void TextBoxDefend_OnKeyUp(object sender, KeyEventArgs e)
    {
      TextBox textbox = (TextBox) sender;
      textbox.Text = e.Key.ToString();
      _settingsHandler.Config.KeyDefend = e.Key;
      _settingsHandler.Save();
    }

    private void TextBoxJump_OnKeyUp(object sender, KeyEventArgs e)
    {
      TextBox textbox = (TextBox) sender;
      textbox.Text = e.Key.ToString();
      _settingsHandler.Config.KeyJump = e.Key;
      _settingsHandler.Save();
    }

    private void TextBoxAttack_OnKeyUp(object sender, KeyEventArgs e)
    {
      TextBox textbox = (TextBox) sender;
      textbox.Text = e.Key.ToString();
      _settingsHandler.Config.KeyAttack = e.Key;
      _settingsHandler.Save();
    }

    private void TextBoxNamePlayer_OnLostFocus(object sender, RoutedEventArgs e)
    {
      TextBox textbox = (TextBox) sender;
      _settingsHandler.Config.NamePlayer = textbox.Text;
      _settingsHandler.Save();
    }

    private void TextBoxPathLf2_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
      TextBox textbox = (TextBox) sender;

      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Filter = "lf2.exe file (*.exe)|*.exe|All files (*.*)|*.*"; ;
      if (openFileDialog.ShowDialog() == true)
        textbox.Text = openFileDialog.FileName;
      _settingsHandler.Config.PathLf2 = textbox.Text;
      _settingsHandler.Save();
    }
  }
}