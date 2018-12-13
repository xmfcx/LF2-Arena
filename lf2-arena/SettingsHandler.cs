﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace lf2_arena
{
  class SettingsHandler
  {
    public class ConfigArena
    {
      public string NamePlayer { get; set; }
      public string PathLf2 { get; set; }
      public Key KeyUp { get; set; }
      public Key KeyDown { get; set; }
      public Key KeyLeft { get; set; }
      public Key KeyRight { get; set; }
      public Key KeyAttack { get; set; }
      public Key KeyJump { get; set; }
      public Key KeyDefend { get; set; }
    }

    public ConfigArena Config;

    public SettingsHandler()
    {
      try
      {
        Load();
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
        Config = new ConfigArena();
      }
    }

    public void Save()
    {
      var serializer = new SerializerBuilder().Build();
      var yaml = serializer.Serialize(Config);

      Debug.WriteLine(yaml);
      File.WriteAllText("config.txt", yaml);
    }

    public void Load()
    {
      var deserializer = new DeserializerBuilder().Build();
      var yaml = File.ReadAllText("config.txt");
      Config = deserializer.Deserialize<ConfigArena>(yaml);
    }
  }
}