using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;

namespace lf2_arena
{
  class Helper
  {
    private static string NameLf2 = "lf2";

    public static void LaunchLf2(int slowerMultipler, string pathFull)
    {
      string startupDir = pathFull;
      NameLf2 = pathFull.Split('\\').Last();
      NameLf2 = NameLf2.Split('.').First();
      Debug.WriteLine(NameLf2);

      for (int i = startupDir.Length - 1; i >= 0; i--)
      {
        if (startupDir[i] == '\\')
        {
          break;
        }
        startupDir = startupDir.Remove(startupDir.Length - 1);
      }


      var lf2Process = new Process();
      if (Process.GetProcessesByName(NameLf2).Length == 0)
      {
        lf2Process.StartInfo.FileName = pathFull;
        lf2Process.StartInfo.WorkingDirectory = startupDir;
        lf2Process.StartInfo.CreateNoWindow = true;
        lf2Process.StartInfo.UseShellExecute = false;
        lf2Process.Start();
      }

      if (Process.GetProcessesByName(NameLf2).Length > 0)
      {
        var proc = Process.GetProcessesByName(NameLf2)[0];
        Thread.Sleep(1000);
        IntPtr handle = proc.MainWindowHandle;
        HookHandler.SetForegroundWindow(handle);
        Thread.Sleep(200);
        Point clientPoint = new Point(400, 250);
        HookHandler.FreezeMouse();
        HookHandler.ClientToScreen(handle, ref clientPoint);
        var oldPos = HookHandler.GetMousePosition();
        HookHandler.SetCursorPos(clientPoint);
        HookHandler.ClickOnPoint(handle, new Point(400, 250));
        Thread.Sleep(500);
        HookHandler.SetCursorPos(oldPos);
        HookHandler.ThawMouse();
        HookHandler.WriteMemWow(proc, 0x44D064, 3, 8);
        var ip_localhost = "127.0.0.1";
        for (int i = 0; i < ip_localhost.Length; i++)
        {
          HookHandler.WriteMem(proc, 0x4592D8 + i, Convert.ToInt32(ip_localhost[i]));
        }

        HookHandler.Input enterInput = new HookHandler.Input();
        enterInput.Data.Keyb.wVk = 0x0D;
        enterInput.Type = 1;
        var inputs = new[] {enterInput};
        HookHandler.SendInput((uint) inputs.Length, inputs, Marshal.SizeOf(typeof(HookHandler.Input)));
        if (Process.GetProcessesByName("lf2-arena").Length > 0)
        {
          Thread.Sleep(200 + slowerMultipler * 100);
          HookHandler.SetForegroundWindow(Process.GetProcessesByName("lf2-arena")[0].MainWindowHandle);
        }

        /*
        Cursor.Position = clientPoint;
        SetPort(Process.GetProcessesByName("lf2")[0].Handle, (IntPtr)0x4546F0, 400);    //x
        SetPort(Process.GetProcessesByName("lf2")[0].Handle, (IntPtr)0x453CDC, 250);    //y
        Cursor.Position = clientPoint;
        WriteMem(Process.GetProcessesByName("lf2")[0], 0x457580, 1);
                        Thread.Sleep(200);
                        SetForegroundWindow(Process.GetProcessesByName("lf2")[0].MainWindowHandle);
                        ClickOnPoint(Process.GetProcessesByName("lf2")[0].MainWindowHandle, new Point(400, 250));
                        Thread.Sleep(50);
                        ClickOnPoint(Process.GetProcessesByName("lf2")[0].MainWindowHandle, new Point(400, 310));
                        Thread.Sleep(50);
                        INPUT[] wololo = new INPUT[10];
                        for (int a = 0; a < wololo.Length; a++)
                                wololo[a].Type = 1;
                        wololo[0].Data.Keyb.wVk = 0x31;
                        wololo[1].Data.Keyb.wVk = 0x32;
                        wololo[2].Data.Keyb.wVk = 0x37;
                        wololo[3].Data.Keyb.wVk = 0xBE;
                        wololo[4].Data.Keyb.wVk = 0x30;
                        wololo[5].Data.Keyb.wVk = 0xBE;
                        wololo[6].Data.Keyb.wVk = 0x30;
                        wololo[7].Data.Keyb.wVk = 0xBE;
                        wololo[8].Data.Keyb.wVk = 0x31;
                        wololo[9].Data.Keyb.wVk = 0x0D;
                        for (int a = 0; a < wololo.Length; a++)
                        {
                                var inputs = new INPUT[] { wololo[a] };
                                SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
                                Thread.Sleep(50);
                        }
                        //SendInput(1,)
                        */
      }
    }

    public static void KillLf2()
    {
      while (Process.GetProcessesByName(NameLf2).Length != 0)
      {
        if (Process.GetProcessesByName(NameLf2).Length > 0)
        {
          try
          {
            for (int a = 0; a < Process.GetProcessesByName(NameLf2).Length; a++)
            {
              Process.GetProcessesByName(NameLf2)[a].Kill();
            }
          }
          catch
          {
          }
        }
      }
    }
  }
}