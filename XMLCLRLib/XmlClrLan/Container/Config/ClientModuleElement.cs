using System;
using System.Xml;
using System.Xml.Serialization;

namespace XmlClrLan
{
    [Serializable]
    public class ClientModuleElement
    {
        /// <summary>
        /// 模块命名标志
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 模块别名标致（使用;分隔多个)
        /// </summary>
        [XmlAttribute(AttributeName = "aliasName")]
        public string AliasName { get; set; }

        /// <summary>
        /// 模块类型
        /// </summary>
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlElement(ElementName = "Build")]
        public SubModuleBuildElement[] MoubleBuild { get; set; }
    }
}
