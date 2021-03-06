﻿using System;
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
  class HostHandler
  {
    public HostHandler()
    {
      var client = new TcpClient();
      client.Connect(IPAddress.Parse("127.0.0.1"), 30100);
      var streamHost = client.GetStream();
      
      MessageArena.Send(streamHost, "a");

      var messageStr = MessageArena.ReceiveString(streamHost);
      Debug.WriteLine(messageStr);

      client.Close();
    }
  }
}
