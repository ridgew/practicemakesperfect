using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;


namespace Common
{
    //https://blog.csdn.net/FairyStepWGL/article/details/51315841

    public class BardCodeHook
    {
        public delegate void BardCodeDeletegate(BarCodes barCode);

        public event BardCodeDeletegate BarCodeEvent;

        public struct BarCodes
        {
            /// <summary>
            /// 虚拟码
            /// </summary>
            public int VirtKey;
            /// <summary>
            /// 扫描码
            /// </summary>
            public int ScanCode;
            /// <summary>
            /// 键名
            /// </summary>
            public string KeyName;
            /// <summary>
            /// Ascll
            /// </summary>
            public uint Ascll;
            /// <summary>
            /// 字符
            /// </summary>
            public char Chr;
            /// <summary>
            /// 条码信息
            /// </summary>
            public string BarCode;
            /// <summary>
            /// 条码是否有效
            /// </summary>
            public bool IsValid;
            /// <summary>
            /// 扫描时间
            /// </summary>
            public DateTime Time;
        }


        private struct EventMsg
        {
            public int message;
            public int paramL;
            public int paramH;
            public int Time;
            public int hwnd;
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);


        [DllImport("user32", EntryPoint = "GetKeyNameText")]
        private static extern int GetKeyNameText(int IParam, StringBuilder lpBuffer, int nSize);


        [DllImport("user32", EntryPoint = "GetKeyboardState")]
        private static extern int GetKeyboardState(byte[] pbKeyState);


        [DllImport("user32", EntryPoint = "ToAscii")]
        private static extern bool ToAscii(int VirtualKey, int ScanCode, byte[] lpKeySate, ref uint lpChar, int uFlags);


        delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        BarCodes barCode = new BarCodes();
        int hKeyboardHook = 0;
        string strBarCode = "";

        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            if (nCode == 0)
            {
                EventMsg msg = (EventMsg)Marshal.PtrToStructure(lParam, typeof(EventMsg));
                if (wParam == 0x100)//WM_KEYDOWN=0x100
                {
                    barCode.VirtKey = msg.message & 0xff;//虚拟吗
                    barCode.ScanCode = msg.paramL & 0xff;//扫描码
                    StringBuilder strKeyName = new StringBuilder(225);
                    if (GetKeyNameText(barCode.ScanCode * 65536, strKeyName, 255) > 0)
                    {
                        barCode.KeyName = strKeyName.ToString().Trim(new char[] { ' ', '\0' });
                    }
                    else
                    {
                        barCode.KeyName = "";
                    }
                    byte[] kbArray = new byte[256];
                    uint uKey = 0;
                    GetKeyboardState(kbArray);

                    if (ToAscii(barCode.VirtKey, barCode.ScanCode, kbArray, ref uKey, 0))
                    {
                        barCode.Ascll = uKey;
                        barCode.Chr = Convert.ToChar(uKey);
                    }


                    TimeSpan ts = DateTime.Now.Subtract(barCode.Time);
                    if (ts.TotalMilliseconds > 50)
                    {
                        strBarCode = barCode.Chr.ToString();
                    }
                    else
                    {
                        if ((msg.message & 0xff) == 13 && strBarCode.Length > 3)
                        {
                            barCode.BarCode = strBarCode;
                            barCode.IsValid = true;
                        }
                        strBarCode += barCode.Chr.ToString();
                    }
                    barCode.Time = DateTime.Now;
                    if (BarCodeEvent != null)
                        BarCodeEvent(barCode);//触发事件
                    barCode.IsValid = false;
                }
            }
            return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }


        //安装钩子
        public bool Start()
        {
            if (hKeyboardHook == 0)
            {
                //WH_KEYBOARD_LL=13
                hKeyboardHook = SetWindowsHookEx(13, new HookProc(KeyboardHookProc),
                    Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
            }
            return (hKeyboardHook != 0);
        }

        //卸载钩子
        public bool Stop()
        {
            if (hKeyboardHook != 0)
            {
                return UnhookWindowsHookEx(hKeyboardHook);
            }
            return true;
        }
    }
}
