using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace lf2_arena
{
  internal class HostHandler
  {
    public HostHandler()
    {
      var client = new TcpClient();
      client.Connect(IPAddress.Parse("127.0.0.1"), 30100);
      var streamHost = client.GetStream();

      MessageArena.Send(streamHost, "register\tmfc\tasdasd");
      var messageStr = MessageArena.ReceiveString(streamHost);
      Debug.WriteLine(messageStr);
      if (messageStr != "success")
      {
        
      }


      client.Close();
    }
  }
}