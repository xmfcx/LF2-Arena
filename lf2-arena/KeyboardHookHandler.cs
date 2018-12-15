using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace lf2_arena
{
  class KeyboardHookHandler
  {
    private static List<Key> _arenaKeys;

    private static readonly StringBuilder KeySet = new StringBuilder("0000000");

    public static void SetKeys(List<Key> arenaKeys)
    {
      _arenaKeys = arenaKeys;
    }

    // Beautiful event system :>
    public delegate void DgEventRaiser(string keySet);

    public static event DgEventRaiser OnKeyEventOccured;

    private static IntPtr _keyboardHook;
    private static readonly LowLevelKeyboardProc suchProc = HookCallback;

    public static void SetIt()
    {
      _keyboardHook = SetHook(suchProc);
    }


    private delegate IntPtr LowLevelKeyboardProc(
      int nCode, IntPtr keyState, IntPtr lParam);


    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
      LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString,
      int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
      IntPtr wParam, IntPtr lParam);


    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
      using (Process curProcess = Process.GetCurrentProcess())
      using (ProcessModule curModule = curProcess.MainModule)
      {
        return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
          GetModuleHandle(curModule.ModuleName), 0);
      }
    }


    private static IntPtr HookCallback(int nCode, IntPtr keyState, IntPtr lParam)
    {
      if (nCode >= 0 && keyState == (IntPtr) WM_KEYDOWN)
      {
        IntPtr hwnd2 = GetForegroundWindow();
        StringBuilder windowtitle = new StringBuilder(256);
        if (GetWindowText(hwnd2, windowtitle, windowtitle.Capacity) > 0)
          if (windowtitle.ToString() == "Little Fighter 2")
          {
            Key key = KeyInterop.KeyFromVirtualKey(Marshal.ReadInt32(lParam));

            for (int i = 0; i < _arenaKeys.Count; i++)
            {
              if (key == _arenaKeys[i])
              {
                KeySet[i] = '1';
              }
            }
            OnKeyEventOccured?.Invoke(KeySet.ToString());
          }
      }

      if (nCode >= 0 && keyState == (IntPtr) WM_KEYUP)
      {
        IntPtr hwnd2 = GetForegroundWindow();
        StringBuilder windowtitle = new StringBuilder(256);
        if (GetWindowText(hwnd2, windowtitle, windowtitle.Capacity) > 0)
          if (windowtitle.ToString() == "Little Fighter 2")
          {
            Key key = KeyInterop.KeyFromVirtualKey(Marshal.ReadInt32(lParam));
            for (int i = 0; i < _arenaKeys.Count; i++)
            {
              if (key == _arenaKeys[i])
              {
                KeySet[i] = '0';
              }
            }
            OnKeyEventOccured?.Invoke(KeySet.ToString());
          }
      }

      return CallNextHookEx(_keyboardHook, nCode, keyState, lParam);
    }
  }
}