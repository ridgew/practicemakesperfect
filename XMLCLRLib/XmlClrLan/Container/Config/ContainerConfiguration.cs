using System;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.Serialization;

namespace XmlClrLan
{
    [Serializable]
    [XmlRoot(ElementName = "ClientModuleContainer", Namespace = "")]
    public class ContainerConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerConfiguration"/> class.
        /// </summary>
        static ContainerConfiguration()
        {
            if (configVals.Count == 0)
            {
                configVals.Add("Constructor", typeof(ModuleConstructorElement).FullName);
                configVals.Add("Call", typeof(ModuleMethodCallElement).FullName);
                configVals.Add("Set", typeof(ModulePropertyElement).FullName);
                configVals.Add("InstanceMethod", typeof(InstanceMethodGetElement).FullName);
                configVals.Add("Binding", typeof(ModuleFieldElement).FullName);
                configVals.Add("return", typeof(ExpressionReturnElement).FullName);
            }
        }

        [XmlElement(ElementName = "Module")]
        public ClientModuleElement[] Modules { get; set; }

        /// <summary>
        /// 已知类型配置
        /// </summary>
        static readonly NameValueCollection configVals = new NameValueCollection(StringComparer.Ordinal);

        /// <summary>
        /// 根据节点名称判断是否是识别类型
        /// </summary>
        public static bool IsKnownTaskByElementName(string elementName, ref Type knownType)
        {
            string configVal = configVals[elementName];
            if (string.IsNullOrEmpty(configVal))
                return false;
            knownType = TypeCache.GetRuntimeType(configVal);
            return true;
        }

    }

}
