using DBHR.ClientModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBHR.WinFormUnity
{
    public class TestModule1 : ClientModuleObj
    {
        public TestModule1()
        {
            FunID = "TestModule1";
        }
    }

    public class TestModule2 : ClientModuleObj
    {
        public TestModule2()
        {
            FunID = "TestModule2";
        }
    }

    public class TestModuleDetail1 : ClientModuleDetailObj
    {
        public TestModuleDetail1()
        {
            FunID = "TestModuleDetail1";
        }
    }

    public class TestModuleDetail2 : ClientModuleDetailObj
    {
        public TestModuleDetail2()
        {
            FunID = "TestModuleDetail2";
        }
    }
}
