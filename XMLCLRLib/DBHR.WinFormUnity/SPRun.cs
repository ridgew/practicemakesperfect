using DBHR.ClientInterface;
using DBHR.ClientModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DBHR.WinFormUnity
{
    public class SPRun : IModuleRun
    {
        public static Action<string, string> ActionTest = null;

        public void RunModule(string moduleId, Form mainForm, string customWhere)
        {
            if (moduleId == "SP1010")
            {
                BaseEditForm form = new BaseEditForm();
                form.AddMainType(typeof(TestModule1));
                form.AddMainType(typeof(TestModule2));

                form.AddDetailType(typeof(TestModule1), typeof(TestModule2));

                form.DoCreateFormControls();
                form.MdiParent = mainForm;
                form.Show();
                return;

            }
        }

        public static void TestMethod2(string arg1, string arg2)
        {
            System.Diagnostics.Trace.WriteLine(arg1);
        }
    }
}
