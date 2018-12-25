using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace lf2_arena
{
  class MessageArena
  {
    private static byte[] ToMessage(string str)
    {
      byte[] payload = Encoding.ASCII.GetBytes(str);
      return ToMessage(payload);
    }

    private static byte[] CombineBytes(byte[] a, byte[] b)
    {
      byte[] rv = new byte[a.Length + b.Length];
      Buffer.BlockCopy(a, 0, rv, 0, a.Length);
      Buffer.BlockCopy(b, 0, rv, a.Length, b.Length);
      return rv;
    }

    private static byte[] ToMessage(byte[] bytes)
    {
      byte[] lenghtBytes = BitConverter.GetBytes((int)bytes.Length);
      return CombineBytes(lenghtBytes, bytes);
    }

    public static void Send(NetworkStream stream, byte[] payload)
    {
      var message = ToMessage(payload);
      stream.Write(message, 0, message.Length);
    }

    public static void Send(NetworkStream stream, string payload)
    {
      var message = ToMessage(payload);
      stream.Write(message, 0, message.Length);
    }

    public static byte[] Receive(NetworkStream stream)
    {
      var message = new byte[4];
      stream.Read(message, 0, message.Length);
      int lengthPayload = BitConverter.ToInt32(message, 0);
      message = new byte[lengthPayload];
      stream.Read(message, 0, message.Length);
      return message;
    }

    public static string ReceiveString(NetworkStream stream)
    {
      return Encoding.ASCII.GetString(Receive(stream));
    }
  }
}