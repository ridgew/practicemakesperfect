﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace DllExportDemo
{
    [SuppressUnmanagedCodeSecurity]
    public static class Indicators
    {

        [DllImport("shell32.dll")]
        private static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LocalFree(IntPtr hMem);

        /*
         * 使 用rundll32.exe运行dll函数
         * https://blog.csdn.net/anda0109/article/details/40111997?_t_t_t=0.345162875120437
         * https://support.microsoft.com/zh-tw/help/164787/info-windows-rundll-and-rundll32-interface
         * https://support.microsoft.com/zh-cn/help/164787/info-windows-rundll-and-rundll32-interface
         void CALLBACK EntryPoint(HWND hwnd, HINSTANCE hinst, LPSTR lpszCmdLine, int nCmdShow);

          RUNDLL.EXE <dllname>,<entrypoint> <optional arguments>

          void CALLBACK RundllFuncExample(HWND hwnd, HINSTANCE hinst, LPSTR lpszCmdLine, int nCmdShow)
            {
             USES_CONVERSION;
             if (lpszCmdLine)
             {
              int nArgs = 0;
              // 这里用了shell api +_+ 关于这个函数请查看MSDN
              LPWSTR* szArglist = ::CommandLineToArgvW(A2W(lpszCmdLine), &nArgs);
              if (NULL != szArglist
               && nArgs == REGAPP_ARG_NUM )
              {
               // 调用真正的dll函数
              }
             }
            }

         RUNDLL32.EXE "example.dll",RundllFuncExample arg1 arg2 arg 3 arg 4 arg 5

        */

        //https://www.cnblogs.com/herenzhiming/articles/6688804.html
        //C#与C/C++的交互（PInvoke）
        //https://raw.githubusercontent.com/cymheart/Sc/e078a1c44319ecc84a5ed54246ba8e38739854be/Sc/Utils/System/ExWindowsAPI.cs

        // http://www.pinvoke.net/default.aspx/shell32/commandlinetoargvw.html
        // Here's an wrapper to CommandLineToArgvW I found useful (csells@sellsbrothers.com)
        public static string[] CommandLineToArgv()
        {
            return CommandLineToArgv(Environment.CommandLine);
        }

        public static string[] CommandLineToArgv(string cmdline)
        {
            if (String.IsNullOrEmpty(cmdline))
            {
                return new string[0];
            }

            var args = new List<string>();
            var argvPtr = IntPtr.Zero;

            try
            {
                int argc;
                argvPtr = CommandLineToArgvW(cmdline, out argc);

                // argvPtr is a pointer to a pointer; dereference it
                var argPtr = Marshal.ReadIntPtr(argvPtr);

                // CommandLineToArgvW will list the executable as argv[0]
                for (var i = 0; i < argc; i++)
                {
                    var arg = Marshal.PtrToStringUni(argPtr);
                    if (arg == null)
                    {
                        continue;
                    }

                    args.Add(arg);

                    // Increment the pointer address by the number of Unicode bytes
                    // plus one Unicode character for the string's null terminator
                    var unicodeByteCount = Encoding.Unicode.GetByteCount(arg) + Encoding.Unicode.GetByteCount(new[] { Char.MinValue });
                    argPtr = new IntPtr(argPtr.ToInt32() + unicodeByteCount);
                }
            }
            catch
            {
                return args.ToArray();
            }
            finally
            {
                LocalFree(argvPtr);
            }

            return args.ToArray();
        }


        [DllImport("user32.dll")]
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

        #region C#调用C++的DLL搜集整理的所有数据类型转换方式,可能会有重复或者多种方案,自己多测试
        //https://www.cnblogs.com/blackice/archive/2013/05/23/3094653.html
        //c++:HANDLE(void *) ---- c#:System.IntPtr  
        //c++:Byte(unsigned char) ---- c#:System.Byte  
        //c++:SHORT(short) ---- c#:System.Int16  
        //c++:WORD(unsigned short) ---- c#:System.UInt16  
        //c++:INT(int) ---- c#:System.Int16
        //c++:INT(int) ---- c#:System.Int32  
        //c++:UINT(unsigned int) ---- c#:System.UInt16
        //c++:UINT(unsigned int) ---- c#:System.UInt32
        //c++:LONG(long) ---- c#:System.Int32  
        //c++:ULONG(unsigned long) ---- c#:System.UInt32  
        //c++:DWORD(unsigned long) ---- c#:System.UInt32  
        //c++:DECIMAL ---- c#:System.Decimal  
        //c++:BOOL(long) ---- c#:System.Boolean  
        //c++:CHAR(char) ---- c#:System.Char  
        //c++:LPSTR(char *) ---- c#:System.String  
        //c++:LPWSTR(wchar_t *) ---- c#:System.String  
        //c++:LPCSTR(const char *) ---- c#:System.String  
        //c++:LPCWSTR(const wchar_t *) ---- c#:System.String  
        //c++:PCAHR(char *) ---- c#:System.String  
        //c++:BSTR ---- c#:System.String  
        //c++:FLOAT(float) ---- c#:System.Single  
        //c++:DOUBLE(double) ---- c#:System.Double  
        //c++:VARIANT ---- c#:System.Object  
        //c++:PBYTE(byte *) ---- c#:System.Byte[]  


        //c++:BSTR ---- c#:StringBuilder
        //c++:LPCTSTR ---- c#:StringBuilder
        //c++:LPCTSTR ---- c#:string
        //c++:LPTSTR ---- c#:[MarshalAs(UnmanagedType.LPTStr)] string  
        //c++:LPTSTR 输出变量名 ---- c#:StringBuilder 输出变量名
        //c++:LPCWSTR ---- c#:IntPtr
        //c++:BOOL ---- c#:bool   
        //c++:HMODULE ---- c#:IntPtr   
        //c++:HINSTANCE ---- c#:IntPtr  
        //c++:结构体 ---- c#:public struct 结构体{};  
        //c++:结构体 **变量名 ---- c#:out 变量名 //C#中提前申明一个结构体实例化后的变量名
        //c++:结构体 &变量名 ---- c#:ref 结构体 变量名

        //c++:WORD ---- c#:ushort
        //c++:DWORD ---- c#:uint
        //c++:DWORD ---- c#:int


        //c++:UCHAR ---- c#:int
        //c++:UCHAR ---- c#:byte
        //c++:UCHAR* ---- c#:string
        //c++:UCHAR* ---- c#:IntPtr


        //c++:GUID ---- c#:Guid
        //c++:Handle ---- c#:IntPtr
        //c++:HWND ---- c#:IntPtr
        //c++:DWORD ---- c#:int
        //c++:COLORREF ---- c#:uint

        //c++:unsigned char ---- c#:byte
        //c++:unsigned char * ---- c#:ref byte
        //c++:unsigned char * ---- c#:[MarshalAs(UnmanagedType.LPArray)] byte[]
        //c++:unsigned char * ---- c#:[MarshalAs(UnmanagedType.LPArray)] Intptr


        //c++:unsigned char & ---- c#:ref byte
        //c++:unsigned char 变量名 ---- c#:byte 变量名
        //c++:unsigned short 变量名 ---- c#:ushort 变量名
        //c++:unsigned int 变量名 ---- c#:uint 变量名
        //c++:unsigned long 变量名 ---- c#:ulong 变量名


        //c++:char 变量名 ---- c#:byte 变量名 //C++中一个字符用一个字节表示,C#中一个字符用两个字节表示
        //c++:char 数组名[数组大小] ---- c#:MarshalAs(UnmanagedType.ByValTStr, SizeConst = 数组大小)] public string 数组名; ushort


        //c++:char * ---- c#:string //传入参数
        //c++:char * ---- c#:StringBuilder//传出参数
        //c++:char *变量名 ---- c#:ref string 变量名
        //c++:char *输入变量名 ---- c#:string 输入变量名
        //c++:char *输出变量名 ---- c#:[MarshalAs(UnmanagedType.LPStr)] StringBuilder 输出变量名


        //c++:char ** ---- c#:string
        //c++:char **变量名 ---- c#:ref string 变量名
        //c++:const char * ---- c#:string
        //c++:char[] ---- c#:string
        //c++:char 变量名[数组大小] ---- c#:[MarshalAs(UnmanagedType.ByValTStr,SizeConst=数组大小)] public string 变量名;  


        //c++:struct 结构体名 *变量名 ---- c#:ref 结构体名 变量名
        //c++:委托 变量名 ---- c#:委托 变量名

        //c++:int ---- c#:int
        //c++:int ---- c#:ref int
        //c++:int & ---- c#:ref int
        //c++:int * ---- c#:ref int //C#中调用前需定义int 变量名 = 0;


        //c++:*int ---- c#:IntPtr
        //c++:int32 PIPTR * ---- c#:int32[]
        //c++:float PIPTR * ---- c#:float[]

        //c++:double** 数组名 ---- c#:ref double 数组名
        //c++:double*[] 数组名 ---- c#:ref double 数组名
        //c++:long ---- c#:int
        //c++:ulong ---- c#:int

        //c++:UINT8 * ---- c#:ref byte //C#中调用前需定义byte 变量名 = new byte();   

        //c++:handle ---- c#:IntPtr
        //c++:hwnd ---- c#:IntPtr

        //c++:void * ---- c#:IntPtr   
        //c++:void * user_obj_param ---- c#:IntPtr user_obj_param
        //c++:void * 对象名称 ---- c#:([MarshalAs(UnmanagedType.AsAny)]Object 对象名称

        //c++:char, INT8, SBYTE, CHAR ---- c#:System.SByte   
        //c++:short, short int, INT16, SHORT ---- c#:System.Int16   
        //c++:int, long, long int, INT32, LONG32, BOOL , INT ---- c#:System.Int32   
        //c++:__int64, INT64, LONGLONG ---- c#:System.Int64   
        //c++:unsigned char, UINT8, UCHAR , BYTE ---- c#:System.Byte   
        //c++:unsigned short, UINT16, USHORT, WORD, ATOM, WCHAR , __wchar_t ---- c#:System.UInt16   
        //c++:unsigned, unsigned int, UINT32, ULONG32, DWORD32, ULONG, DWORD, UINT ---- c#:System.UInt32   
        //c++:unsigned __int64, UINT64, DWORDLONG, ULONGLONG ---- c#:System.UInt64   
        //c++:float, FLOAT ---- c#:System.Single   
        //c++:double, long double, DOUBLE ---- c#:System.Double   


        //Win32 Types ---- CLR Type   
        //Struct需要在C#里重新定义一个Struct
        //CallBack回调函数需要封装在一个委托里，delegate static extern int FunCallBack(string str);

        //unsigned char** ppImage替换成IntPtr ppImage
        //int& nWidth替换成ref int nWidth
        //int*, int&, 则都可用 ref int 对应
        //双针指类型参数，可以用 ref IntPtr
        //函数指针使用c++: typedef double (*fun_type1)(double); 对应 c#:public delegate double fun_type1(double);
        //char* 的操作c++: char*; 对应 c#:StringBuilder;
        //c#中使用指针:在需要使用指针的地方 加 unsafe


        //unsigned char对应public byte
        /*
        * typedef void (*CALLBACKFUN1W)(wchar_t*, void* pArg);
        * typedef void (*CALLBACKFUN1A)(char*, void* pArg);
        * bool BIOPRINT_SENSOR_API dllFun1(CALLBACKFUN1 pCallbackFun1, void* pArg);
        * 调用方式为
        * [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        * public delegate void CallbackFunc1([MarshalAs(UnmanagedType.LPWStr)] StringBuilder strName, IntPtr pArg);
        */
        #endregion

        internal delegate void EntryPoint(IntPtr hwnd, IntPtr hinst, [MarshalAs(UnmanagedType.LPStr)]string lpszCmdLine, int nCmdShow);

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void Main(IntPtr hwnd, IntPtr hinst, [MarshalAs(UnmanagedType.LPStr)]string lpszCmdLine, int nCmdShow)
        {
            if (!string.IsNullOrEmpty(lpszCmdLine))
                MessageBox(hinst, lpszCmdLine, "提示信息", 64);
            else
                MessageBox(hinst, "C# Dll Export Demo", "提示信息", 64);
        }


        //https://docs.mql4.com/cn/indicators/ima
        /// <summary>
        /// Calculates the Moving Average indicator and returns its value.
        /// </summary>
        /// <param name="symbol">symbol</param>
        /// <param name="timeframe">timeframe</param>
        /// <param name="ma_period">MA averaging period</param>
        /// <param name="ma_shift">MA shift</param>
        /// <param name="ma_method">averaging method</param>
        /// <param name="applied_price">applied price</param>
        /// <param name="shift">shift</param>
        /// <returns></returns>
        [DllExport]
        public static double iMa(string symbol, int timeframe, int ma_period, int ma_shift, int ma_method, int applied_price, int shift)
        {
            return Math.PI;
        }

        [DllExport]
        public static bool getIBandsOfPeriod(string symb, int period, ref double[] retVal, int shift = 0)
        {
            //retVal[0] = iBands(symb, period, iBandsPeriod, iBandsDeviation, 0, iBandsPrice, MODE_LOWER, shift);
            //retVal[1] = iBands(symb, period, iBandsPeriod, iBandsDeviation, 0, iBandsPrice, MODE_MAIN, shift);
            //retVal[2] = iBands(symb, period, iBandsPeriod, iBandsDeviation, 0, iBandsPrice, MODE_UPPER, shift);
            return true;
        }

        /// <summary>
        ///  iBands, Calculates the Bollinger Bands® indicator and returns its value.
        /// </summary>
        /// <param name="symbol">symbol</param>
        /// <param name="timeframe">timeframe</param>
        /// <param name="period">averaging period </param>
        /// <param name="deviation">standard deviations </param>
        /// <param name="bands_shift">bands shift </param>
        /// <param name="applied_price">applied price </param>
        /// <param name="mode">line index</param>
        /// <param name="shift">shift</param>
        /// <returns></returns>
        [DllExport]
        public static double iBands(string symbol, int timeframe, int period, double deviation, int bands_shift, int applied_price, int mode, int shift)
        {
            return Math.PI;
        }



        [DllExport]
        public static bool getStoOfPeriod(string symb, int period, ref double[] retVal, int shift = 0)
        {
            //retVal[0] = iStochastic(symb, period, kdj_k, kdj_d, kdj_slow, MODE_SMA, 0, MODE_SIGNAL,shift);
            //retVal[1] = iStochastic(symb, period, kdj_k, kdj_d, kdj_slow, MODE_SMA, 0, MODE_MAIN, shift);
            return true;
        }

        /// <summary>
        /// iStochastic ,Calculates the Stochastic Oscillator and returns its value.
        /// </summary>
        /// <param name="symbol">symbol</param>
        /// <param name="timeframe">timeframe</param>
        /// <param name="Kperiod">K line period </param>
        /// <param name="Dperiod">D line period </param>
        /// <param name="slowing">slowing </param>
        /// <param name="method">averaging method </param>
        /// <param name="price_field"> price (Low/High or Close/Close) </param>
        /// <param name="mode">line index </param>
        /// <param name="shift">shift</param>
        /// <returns></returns>
        [DllExport]
        public static double iStochastic(string symbol, int timeframe, int Kperiod, int Dperiod, int slowing, int method, int price_field, int mode, int shift)
        {
            return Math.PI;
        }

    }
}
