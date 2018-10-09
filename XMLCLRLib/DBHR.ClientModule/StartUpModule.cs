using DBHR.ClientInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XmlClrLan;

namespace DBHR.ClientModule
{
    public class StartUpModule : IModuleRun
    {
        /// <summary>
        /// 执行模块调用逻辑
        /// </summary>
        /// <param name="moduleId">模块完整编号</param>
        /// <param name="mainForm">窗体容器</param>
        /// <param name="customWhere">附加条件</param>
        /// <exception cref="NotImplementedException"></exception>
        public void RunModule(string moduleId, Form mainForm, string customWhere)
        {
            using (ModuleRunScope scope = new ModuleRunScope())
            {
               IModuleRun run = ClientMoudleContainer.Instance.Resolve<IModuleRun>(moduleId);
            }  
        }
    }
}
