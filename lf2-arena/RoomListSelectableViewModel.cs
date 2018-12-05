using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace lf2_arena
{
  class RoomListSelectableViewModel : INotifyPropertyChanged
  {
    private string _playerName;
    private int _playerPing;
    private int _playerNumber;
    private string _playerMode;

    public int PlayerNumber
    {
      get { return _playerNumber; }
      set
      {
        if (_playerNumber == value) return;
        _playerNumber = value;
        OnPropertyChanged();
      }
    }

    public string PlayerName
    {
      get { return _playerName; }
      set
      {
        if (_playerName == value) return;
        _playerName = value;
        OnPropertyChanged();
      }
    }

    public int PlayerPing
    {
      get { return _playerPing; }
      set
      {
        if (_playerPing == value) return;
        _playerPing = value;
        OnPropertyChanged();
      }
    }

    public string PlayerMode
    {
      get { return _playerMode; }
      set
      {
        if (_playerMode == value) return;
        _playerMode = value;
        OnPropertyChanged();
      }
    }


    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}