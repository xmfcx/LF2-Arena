using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Input;

namespace lf2_arena
{
  class HookHandler
  {
    public const int WH_KEYBOARD_LL = 13;
    public const int WM_KEYDOWN = 0x0100;
    public const int WM_KEYUP = 0x0101;
    public static LowLevelKeyboardProc SuchProc = HookCallback;
    public static IntPtr KeyboardHook = IntPtr.Zero;

    public static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
      using (Process curProcess = Process.GetCurrentProcess())
      using (ProcessModule curModule = curProcess.MainModule)
      {
        return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
          GetModuleHandle(curModule.ModuleName), 0);
      }
    }

    public delegate IntPtr LowLevelKeyboardProc(
      int nCode, IntPtr keyState, IntPtr lParam);

    public static IntPtr HookCallback(
      int nCode, IntPtr keyState, IntPtr lParam)
    {
      if (nCode >= 0 && keyState == (IntPtr) WM_KEYDOWN)
      {
        IntPtr hwnd2 = GetForegroundWindow();
        StringBuilder windowtitle = new StringBuilder(256);
        if (GetWindowText(hwnd2, windowtitle, windowtitle.Capacity) > 0)
          if ((windowtitle.ToString() == "Little Fighter 2"))
          {
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
          if ((windowtitle.ToString() == "Little Fighter 2"))
          {
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

      return CallNextHookEx(KeyboardHook, nCode, keyState, lParam);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(int idHook,
      LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
      IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString,
      int nMaxCount);

    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(Key vKey);

    //EditLf2Mem[Flags]
    public enum ProcessAccessFlags : uint
    {
      All = 0x001F0FFF,
    }

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
      [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize,
      out int lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    public static extern Int32 CloseHandle(IntPtr hProcess);

    public static void WriteMem(Process p, int address, long v)
    {
      var hProcWow = OpenProcess(ProcessAccessFlags.All, false, p.Id);
      var val = new[] {(byte) v};
      int wtf;
      WriteProcessMemory(hProcWow, new IntPtr(address), val, (UInt32) val.LongLength, out wtf);
      CloseHandle(hProcWow);
    }

    public static void WriteMemWow(Process p, int address, long v, int length)
    {
      var hProcWow = OpenProcess(ProcessAccessFlags.All, false, p.Id);
      var val = new[] {(byte) v};
      int wtf;
      WriteProcessMemory(hProcWow, new IntPtr(address), val, (UInt32) length, out wtf);
      CloseHandle(hProcWow);
    }

    //Better Memory Writer
    public static void SetPort(IntPtr gameHandle, IntPtr writeAddress, int i)
    {
      var array = BitConverter.GetBytes(i);
      int bytesWritten;
      WriteProcessMemory(gameHandle, writeAddress, array, (uint) array.Length, out bytesWritten);
    }

    [Flags]
    public enum ThreadAccess
    {
      SUSPEND_RESUME = (0x0002),
    }

    [DllImport("kernel32.dll")]
    static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

    [DllImport("kernel32.dll")]
    static extern uint SuspendThread(IntPtr hThread);

    [DllImport("kernel32.dll")]
    static extern int ResumeThread(IntPtr hThread);

    public static void SuspendProcess(Process process)
    {
      if (process.ProcessName == string.Empty)
        return;
      foreach (ProcessThread pT in process.Threads)
      {
        IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint) pT.Id);
        if (pOpenThread == IntPtr.Zero)
        {
          continue;
        }

        SuspendThread(pOpenThread);
        CloseHandle(pOpenThread);
      }
    }

    public static void ResumeProcess(Process process)
    {
      if (process.ProcessName == string.Empty)
        return;
      foreach (ProcessThread pT in process.Threads)
      {
        IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint) pT.Id);
        if (pOpenThread == IntPtr.Zero)
        {
          continue;
        }

        int suspendCount;
        do
        {
          suspendCount = ResumeThread(pOpenThread);
        } while (suspendCount > 0);

        CloseHandle(pOpenThread);
      }
    }

    [DllImport("user32.dll")]
    public static extern bool BlockInput(bool block);

    public static void FreezeMouse()
    {
      BlockInput(true);
    }

    public static void ThawMouse()
    {
      BlockInput(false);
    }

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

    [DllImport("user32.dll")]
    internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] Input[] pInputs,
      int cbSize);

    internal struct Input
    {
      public UInt32 Type;
      public MOUSEKEYBDHARDWAREINPUT Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct MOUSEKEYBDHARDWAREINPUT
    {
      [FieldOffset(0)] public MOUSEINPUT Mouse;
      [FieldOffset(0)] public KEYBDINPUT Keyb;
    }

    internal struct KEYBDINPUT
    {
      public int wVk;
      public int wScan;
      public int dwFlags;
      public int time;
      public IntPtr dwExtraInfo;
    }

    internal struct MOUSEINPUT
    {
      public Int32 X;
      public Int32 Y;
      public UInt32 MouseData;
      public UInt32 Flags;
      public UInt32 Time;
      public IntPtr ExtraInfo;
    }

    public static void ClickOnPoint(IntPtr wndHandle, Point clientPoint)
    {
      var oldPos = GetMousePosition();
      // get screen coordinates
      ClientToScreen(wndHandle, ref clientPoint);
      // set cursor on coords, and press mouse
      SetCursorPos(clientPoint);
      var inputMouseDown = new Input();
      inputMouseDown.Type = 0; // input type mouse
      inputMouseDown.Data.Mouse.Flags = 0x0002; // left button down
      var inputMouseUp = new Input();
      inputMouseUp.Type = 0; // input type mouse
      inputMouseUp.Data.Mouse.Flags = 0x0004; // left button up
      SetCursorPos(clientPoint);
      var inputs = new[] {inputMouseDown};
      SendInput((uint) inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
      // return mouse
      SetCursorPos(oldPos);
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(ref Win32Point pt);

    [StructLayout(LayoutKind.Sequential)]
    internal struct Win32Point
    {
      public Int32 X;
      public Int32 Y;
    };

    public static Point GetMousePosition()
    {
      Win32Point w32Mouse = new Win32Point();
      GetCursorPos(ref w32Mouse);
      return new Point(w32Mouse.X, w32Mouse.Y);
    }

    [DllImport("User32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    public static void SetCursorPos(Point point)
    {
      SetCursorPos((int) point.X, (int) point.Y);
    }
  }
}