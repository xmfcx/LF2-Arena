using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace lf2_arena
{
  class Lf2Handler
  {
    private readonly object _lockKeyString = new object();

    private string _keyString = "0000000";

    public void SetKeyString(string keys)
    {
      lock (_lockKeyString)
      {
        _keyString = keys;
        Console.WriteLine(_keyString);
      }
    }

    // Beautiful event system :>
    public delegate void DgEventRaiser(bool roomState);

    public event DgEventRaiser OnRoomStateChanged;

    public Lf2Handler()
    {
      KeyboardHookHandler.SetIt();
    }

    public void ListenForLf2()
    {
      Thread thread = new Thread(Listen);
      thread.Start();
      thread.IsBackground = true;
    }

    private void Listen()
    {
      var listenerLf2 = new TcpListener(IPAddress.Any, 12345);
      listenerLf2.Start();
      while (true)
      {
        try
        {
          // blocking listening operation
          var client = listenerLf2.AcceptTcpClient();

          // go green
          OnRoomStateChanged?.Invoke(true);

          // blocking communication with the lf2
          Lf2Talker(client);
        }
        catch (Exception)
        {
          // ignored
        }
        // go yellow
        OnRoomStateChanged?.Invoke(false);
      }
    }

    public void Lf2Talker(TcpClient client)
    {
      NetworkStream streamToLobby;
      TcpClient clientLobby;


      client.Close();
    }
  }
}