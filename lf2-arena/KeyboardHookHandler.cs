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


    // Beautiful event system :>
    public delegate void DgEventRaiser(string keySet);
    public static event DgEventRaiser OnKeyEventOccured;


    private static string keySet = "0000000";

    private static IntPtr keyboardHook;
    private static LowLevelKeyboardProc suchProc = HookCallback;

    public static void SetIt()
    {
      keyboardHook = SetHook(suchProc);
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
            OnKeyEventOccured?.Invoke(keySet);
            int vkCode = Marshal.ReadInt32(lParam);
            for (int a = 0; a < 7; a++)
            {
              //if ((Key)vkCode == _lf2Handler.keyPlayer[a])
              //{
              //  _lf2Handler.keyLogic[a] = true;
              //}
            }
          }
      }

      if (nCode >= 0 && keyState == (IntPtr) WM_KEYUP)
      {
        IntPtr hwnd2 = GetForegroundWindow();
        StringBuilder windowtitle = new StringBuilder(256);
        if (GetWindowText(hwnd2, windowtitle, windowtitle.Capacity) > 0)
          if (windowtitle.ToString() == "Little Fighter 2")
          {
            OnKeyEventOccured?.Invoke(keySet);
            int vkCode = Marshal.ReadInt32(lParam);
            for (int a = 0; a < 7; a++)
            {
              //if ((Key)vkCode == _lf2Handler.keyPlayer[a])
              //{
              //  _lf2Handler.keyLogic[a] = false;
              //}
            }
          }
      }

      return CallNextHookEx(keyboardHook, nCode, keyState, lParam);
    }
  }
}