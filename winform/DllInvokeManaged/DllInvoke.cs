using System;
using System.Runtime.InteropServices;

namespace DllInvokeManaged
{
    public class DllInvoke
    {

        #region Win API
        [DllImport("kernel32.dll")]
        private extern static IntPtr LoadLibrary(string path);

        [DllImport("kernel32.dll")]
        private extern static IntPtr GetProcAddress(IntPtr lib, string funcName);

        [DllImport("kernel32.dll")]
        private extern static bool FreeLibrary(IntPtr lib);
        #endregion

        private IntPtr hLib;
        public DllInvoke(String DLLPath)
        {
            hLib = LoadLibrary(DLLPath);
        }

        ~DllInvoke()
        {
            FreeLibrary(hLib);
        }

        //将要执行的函数转换为委托
        public Delegate Invoke(string APIName, Type t)
        {
            IntPtr api = GetProcAddress(hLib, APIName);
            if (api != IntPtr.Zero)
               return (Delegate)Marshal.GetDelegateForFunctionPointer(api, t);
            return null;
        }

    }
}
