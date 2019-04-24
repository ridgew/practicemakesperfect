using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DllInvokeManaged
{
    class Program
    {
        public delegate string Test(string a);

        [DllImport("test2.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern string testC(string str);

        static void Main(string[] args)
        {
            DllInvoke dll = new DllInvoke("test2.dll");
            Test test = (Test)dll.Invoke("testC", typeof(Test));
            if (test != null)
            {
                string s = test("abc中文");
                Console.WriteLine(s);
            }
            else
            {
                string s = testC("abc中文");
                Console.WriteLine(s);
            }

            Console.WriteLine("****************************************");
            Console.ReadLine();

        }
    }
}
