using System;
using System.Windows.Forms;

namespace USBHookTes
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new USBKeyboard());
            Application.Run(new MultiFormApplictionStart(typeof(WinForm_Scaner.Scaner), typeof(USBKeyboard), typeof(COM_TEST.COMForm)));
        }
    }
}
