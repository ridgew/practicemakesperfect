using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace XmlClrLan
{
    public class XmlSerializeSectionHandler : IConfigurationSectionHandler
    {
        // Methods
        public object Create(object parent, object configContext, XmlNode section)
        {
            string typeName = (string)section.CreateNavigator().Evaluate("string(@type)");
            XmlSerializer serializer = new XmlSerializer(Type.GetType(typeName, true));
            return serializer.Deserialize(new XmlNodeReader(section));
        }

        public static T GetObject<T>(string sectionName, string cfgFilePath = @"SysConfig\ClientModule.config")
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                sectionName = typeof(T).Name;
            }

            if (string.IsNullOrEmpty(cfgFilePath))
            {
                //var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = filePath };
                //从config文件中读取配置信息
                //Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                //object section = configuration.GetSection(sectionName);

                object section = ConfigurationManager.GetSection(sectionName);
                if (section != null)
                    return (T)section;
                return default(T);
            }
            else
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cfgFilePath);
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("系统配置错误，序列化配置文件不存在！", filePath);
                XmlDocument cfgDoc = new XmlDocument();
                cfgDoc.Load(filePath);
                return cfgDoc.GetObject<T>();
            }
        }
    }


}
