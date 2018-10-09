using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Unity;

namespace DBHR.WinFormUnity
{
    public class UnityContainerManager
    {
        #region 单例
        private readonly IUnityContainer _unityContainer;
        private UnityContainerManager()
        {
            _unityContainer = LoadUnityConfig();
        }

        private static UnityContainerManager _instance;
        public static UnityContainerManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UnityContainerManager();
                return _instance;
            }
        }
        #endregion

        /// <summary>
        /// 实例化对象
        /// </summary>
        /// <typeparam name="T"> 对象类型 </typeparam>
        /// <returns> 实例对象 </returns>
        public T Resolve<T>()
        {
            return _unityContainer.Resolve<T>();
        }

        /// <summary>
        /// 加载Unity.config配置信息
        /// </summary>
        /// <returns></returns>
        private IUnityContainer LoadUnityConfig()
        {
            ////根据文件名获取指定config文件
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"Configs\Unity.config";
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = filePath };

            //从config文件中读取配置信息
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            var unitySection = (UnityConfigurationSection)configuration.GetSection("unity");
            var container = new UnityContainer();
            foreach (var item in unitySection.Containers)
            {
                container.LoadConfiguration(unitySection, item.Name);
            }

            return container;
        }
    }
}

