using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public HostHandler HostHandler;
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
      HostHandler = new HostHandler();
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
      var streamLf2 = client.GetStream();
      
      streamLf2.Write(Encoding.ASCII.GetBytes("u can connect\0"), 0, 14);
      Thread.Sleep(500);

      var message = new byte[77];
      streamLf2.Read(message, 0, message.Length);
      PrintEnc(message);
      //Print(message);

      for (int i = 0; i < 8; i++)
      {
        message[i] = (byte) '1';
      }
      streamLf2.Write(message, 0, message.Length);

      Random random = new Random();
      byte[] seed = new byte[3001];
      random.NextBytes(seed);
      streamLf2.Write(seed, 0, 3001);

      message = new byte[22];
      while (true)
      {
        streamLf2.Read(message, 0, message.Length);
        string keyString;
        lock (_lockKeyString)
        {
          keyString = _keyString;
        }
        int keySet = 1;
        for (int i = 0; i < keyString.Length; i++)
        {
          if (keyString[i] == '1')
            keySet += Convert.ToInt32(Math.Pow(2, i + 1));
        }
        Debug.WriteLine("key: " + keySet);
        message[0] = Convert.ToByte(keySet);
        streamLf2.Write(message, 0, 22);
      }

      client.Close();
    }


    public static string ByteArrayToString(byte[] ba)
    {
      return BitConverter.ToString(ba).Replace("-", " ");
    }

    public static void Print(byte[] ba)
    {
      Debug.WriteLine(ByteArrayToString(ba));
    }

    public static void PrintEnc(byte[] ba)
    {
      Debug.WriteLine(Encoding.ASCII.GetString(ba, 0, ba.Length));
    }
  }
}