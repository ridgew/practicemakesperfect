using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XmlClrLan;

namespace DBHR.ClientModule
{
    public class ClientMoudleContainer
    {
        static ClientMoudleContainer()
        {
            if (ConfigInstance == null)
                ConfigInstance = XmlSerializeSectionHandler.GetObject<ContainerConfiguration>("ClientModuleContainer", @"SysConfig\ClientModule.config");
        }

        static readonly ContainerConfiguration ConfigInstance = null;

        public static readonly ClientMoudleContainer Instance = new ClientMoudleContainer();

        public TInstanse Resolve<TInstanse>(string moduleId)
        {
            string mainModule = moduleId.Substring(0, 2);
            ClientModuleElement moduleCfg = ConfigInstance.Modules.FirstOrDefault(m => m.Name.Equals(mainModule, StringComparison.InvariantCultureIgnoreCase));
            if (moduleCfg == null)
            {
                throw new System.Configuration.ConfigurationErrorsException("主模块在模块配置中不存在！");
            }

            //TODO 模块生命周期配置
            return ModuleBuilder.BuildAndInvoke<TInstanse>(moduleCfg, moduleId);
        }

        public void ResolveAndInvoke(string mainModule, string moduleId, Form container)
        {
            ClientModuleElement moduleCfg = ConfigInstance.Modules.FirstOrDefault(m => m.Name.Equals(mainModule, StringComparison.InvariantCultureIgnoreCase));
            if (moduleCfg == null)
            {
                throw new System.Configuration.ConfigurationErrorsException("主模块在模块配置中不存在！");
            }

            SubModuleBuildElement sub = moduleCfg.MoubleBuild.FirstOrDefault(s => s.ModuleId == moduleId);
            if (sub == null)
                throw new System.Configuration.ConfigurationErrorsException("子模块在模块配置中不存在！");

            using (ModuleRunScope scope = new ModuleRunScope())
            {
                scope.SetVariable("mainForm", container);
                ModuleBuilder.BuildAndInvoke(sub, scope);
                if (scope.LastError != null)
                {
                    throw scope.LastError;
                }
            }

        }
    }
}
