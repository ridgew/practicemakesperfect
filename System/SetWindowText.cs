using System.Runtime.InteropServices;
using System;

namespace SetWindowText
{
  public class Program
  {
    [DllImport("user32.dll", EntryPoint="FindWindow", SetLastError = true)]
    static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPTStr)] string lParam);


    static int Main(string[] args)
    {
      if( args.Length < 2 )
      {
        return 1;
      }

      IntPtr win = FindWindowByCaption(IntPtr.Zero, args[0]);
      if( win == IntPtr.Zero )
      {
        return 2;
      }

      SendMessage(win, 0xC, IntPtr.Zero, args[1]);
      return 0;
    }
  }
}
