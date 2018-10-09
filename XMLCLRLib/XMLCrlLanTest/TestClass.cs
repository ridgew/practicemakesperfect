using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace XMLCrlLanTest
{
    public class TestClass
    {

        public delegate string MyFuncString(string a, string b);

        public static void TestAction(string arg1, string arg2)
        {
            //Console.WriteLine(arg1);
            Trace.WriteLine(arg1);
        }

        public static MyFuncString FuncString = null;

    }

    public class TestClass2
    {
        static readonly TestClass2 _instance = new TestClass2();

        public static TestClass2 Instance()
        {
            return _instance;
        }

        public string TestDelegate(string a, string b)
        {
            return b;
        }
    }
}
