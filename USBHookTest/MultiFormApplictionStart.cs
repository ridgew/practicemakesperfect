using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace USBHookTes
{
    /// <summary>
    /// 多窗口同时启动类
    /// <remarks>继承ApplicationContext的原因是Application.Run(ApplicationContext context);参数的需要</remarks>
    /// <remarks>另一个是关闭同时启动的窗口</remarks>
    /// </summary>
    public class MultiFormApplictionStart : ApplicationContext
    {

        public MultiFormApplictionStart(params Type[] formTypes)
        {
            var formList = new List<Form>();
            foreach (Type fType in formTypes)
            {
                if (fType.IsSubclassOf(typeof(Form)))
                {
                    formList.Add((Form)Activator.CreateInstance(fType));
                }
            }

            foreach (var item in formList)
            {
                item.FormClosed += onFormClosed;
                item.Show();
            }
        }

        private void onFormClosed(object sender, EventArgs e)
        {
            if (Application.OpenForms.Count == 0)
            {
                ExitThread();
            }
        }

    }
}
