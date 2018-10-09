using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DBHR.ClientInterface
{
    public interface IModuleRun
    {
        /// <summary>
        /// 执行模块调用逻辑
        /// </summary>
        /// <param name="moduleId">模块完整编号</param>
        /// <param name="mainForm">窗体容器</param>
        /// <param name="customWhere">附加条件</param>
        void RunModule(string moduleId, Form mainForm, string customWhere);
    }
}
