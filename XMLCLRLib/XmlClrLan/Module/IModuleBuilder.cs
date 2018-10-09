using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlClrLan
{
    public interface IModuleBuilder
    {
        /// <summary>
        /// 在作用域下构建执行
        /// </summary>
        void BuildAndInvoke(SubModuleBuildElement sub, ModuleRunScope scope);
    }

    /// <summary>
    /// 模块单独执行
    /// </summary>
    public interface IExecutableInScope
    {
        /// <summary>
        /// 在作用域下单独执行
        /// </summary>
        void InvokeInScope(ModuleRunScope scope);
    }
}
