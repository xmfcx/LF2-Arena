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
    int _keySetLast = 1;
    int _keySetSentLast = 1;
    Queue<Tuple<DateTime, int>> _inputQueue = new Queue<Tuple<DateTime, int>>();

    public void SetKeyString(string keys)
    {
      int keySet = 1;
      lock (_lockKeyString)
      {
        _keyString = keys;
        for (int i = 0; i < _keyString.Length; i++)
        {
          if (_keyString[i] == '1')
            keySet += Convert.ToInt32(Math.Pow(2, i + 1));
        }
        if (_keySetLast != keySet)
        {
          _inputQueue.Enqueue(new Tuple<DateTime, int>(DateTime.Now, keySet));
          _keySetLast = keySet;
        }
        Debug.WriteLine("input: " + _keyString);
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

    private void Lf2Talker(TcpClient client)
    {
      var streamLf2 = client.GetStream();
      streamLf2.Write(Encoding.ASCII.GetBytes("u can connect\0"), 0, 14);
      Thread.Sleep(500);

      var message = new byte[77];
      streamLf2.Read(message, 0, message.Length);
      PrintEnc(message);

      for (int i = 0; i < 8; i++)
      {
        message[i] = (byte)'1';
      }
      streamLf2.Write(message, 0, message.Length);

      var seed = GenerateRandomBytes(3001);
      streamLf2.Write(seed, 0, seed.Length);

      var messageNtsd = new byte[16];
      var ntsdStupidMessage = Encoding.ASCII.GetBytes("u can connect\0\0\0");
      while (true)
      {
        // First read exceptional 16 bytes that might belong to ntsd
        streamLf2.Read(messageNtsd, 0, messageNtsd.Length);
        if (messageNtsd.SequenceEqual(ntsdStupidMessage))
        {
          // And handle it if it belongs to ntsd
          streamLf2.Write(messageNtsd, 0, messageNtsd.Length);
          continue;
        }
        // Else, read the rest, combine and keep on.
        var messageRest = new byte[6];
        streamLf2.Read(messageRest, 0, messageRest.Length);
        message = CombineArrays(messageNtsd, messageRest);

        int keySetSend = _keySetSentLast;
        lock (_lockKeyString)
        {
          if (_inputQueue.Count > 0)
          {
            var tuple = _inputQueue.Dequeue();
            var time = tuple.Item1;
            keySetSend = tuple.Item2;
            while (_inputQueue.Count > 0)
            {
              if ((_inputQueue.Peek().Item1 - time).TotalMilliseconds > 50)
                break;

              tuple = _inputQueue.Dequeue();
              time = tuple.Item1;
              keySetSend = tuple.Item2;
            }
          }
        }

        message[0] = Convert.ToByte(keySetSend);
        streamLf2.Write(message, 0, 22);
        _keySetSentLast = keySetSend;
      }

      client.Close();
    }

    private static byte[] GenerateRandomBytes(int count)
    {
      var random = new Random();
      byte[] seed = new byte[count];
      random.NextBytes(seed);
      return seed;
    }

    private static byte[] CombineArrays(byte[] array1, byte[] array2)
    {
      var arrayCombined = new byte[array1.Length + array2.Length];
      Array.Copy(array1, arrayCombined, array1.Length);
      Array.Copy(array2, 0, arrayCombined, array1.Length, array2.Length);
      return arrayCombined;
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