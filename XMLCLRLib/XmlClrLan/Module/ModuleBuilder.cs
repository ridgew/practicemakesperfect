using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XmlClrLan
{
    public class ModuleBuilder
    {
        /// <summary>
        /// 通过节点配置构建对象并执行
        /// </summary>
        /// <typeparam name="TInstance">对象类型</typeparam>
        /// <param name="moduleCfg">The module CFG.</param>
        /// <returns></returns>
        public static TInstance BuildAndInvoke<TInstance>(ClientModuleElement moduleCfg, string subModuleId)
        {
            Type taregetType = TypeCache.GetRuntimeType(moduleCfg.Type);
            TInstance instance = default(TInstance);

            SubModuleBuildElement sub = null;
            if (moduleCfg.MoubleBuild != null && moduleCfg.MoubleBuild.Any())
            {
                sub = moduleCfg.MoubleBuild.FirstOrDefault(m => m.ModuleId == subModuleId);
            }
            return instance;
        }

        /// <summary>
        /// 在模块执行作用域执行（调用）模块代码
        /// </summary>
        /// <param name="sub">构建配置项</param>
        /// <param name="scope">执行作用域</param>
        public static void BuildAndInvoke(SubModuleBuildElement sub, ModuleRunScope scope)
        {
            sub.InvokeInScope(scope);
        }

        /// <summary>
        /// 执行模块语句
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="modules">The modules.</param>
        public static void RunScopeModule(ModuleRunScope scope, params ModuleBlock[] modules)
        {
            foreach (var sub in modules)
            {
                sub.InvokeInScope(scope);
            }
        }
    }
}
