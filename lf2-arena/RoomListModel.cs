using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace lf2_arena
{
  class RoomListModel : INotifyPropertyChanged
  {
    private readonly ObservableCollection<RoomListSelectableViewModel> _roomItems;

    public RoomListModel()
    {
      _roomItems = CreateData();
    }

    private static ObservableCollection<RoomListSelectableViewModel> CreateData()
    {
      return new ObservableCollection<RoomListSelectableViewModel>
      {
        new RoomListSelectableViewModel
        {
          PlayerNumber = 1,
          PlayerName = "Monk",
          PlayerPing = 18,
          PlayerMode = "Waiting"
        },
        new RoomListSelectableViewModel
        {
          PlayerNumber = 2,
          PlayerName = "Wololo",
          PlayerPing = 30,
          PlayerMode = "Connected"
        },
        new RoomListSelectableViewModel
        {
          PlayerNumber = 9,
          PlayerName = "Roggan",
          PlayerPing = 29,
          PlayerMode = "Spectator"
        }
      };
    }

    public ObservableCollection<RoomListSelectableViewModel> RoomItems => _roomItems;
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}