using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace XmlClrLan
{
    [XmlRoot(ElementName = "when")]
    [Serializable]
    public class ModuleWhen : SubModuleBuildElement
    {
        /// <summary>
        /// 运行条件配置(match)
        /// </summary>
        [XmlElement(ElementName = "match")]
        public ConditionalElement Match { get; set; }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象。
        /// </summary>
        /// <param name="reader">从中对对象进行反序列化的 <see cref="T:System.Xml.XmlReader" /> 流。</param>
        public override void ReadXml(XmlReader reader)
        {
            int entDepth = reader.Depth;

            //处理开始节点
            if (reader.NodeType == XmlNodeType.Element && reader.Name == "when")
            {
                reader.ReadToElement("match");
                if (reader.Name == "match")
                {
                    ConditionalElement myWhen = new ConditionalElement();
                    myWhen.ReadXml(reader);
                    Match = myWhen;

                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "match")
                        reader.ReadEndElement();
                }

                reader.ReadToElement("invoke");
                if (reader.Name == "invoke")
                {
                    reader.ReadToElement(null);
                    while (reader.NodeType == XmlNodeType.Element)
                    {
                        _innerSteps.ReadXmlEx<ModuleBuildStepElement>(reader);
                    }
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "invoke")
                        reader.ReadEndElement();
                }

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "when")
                    reader.ReadEndElement();
            }
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 <see cref="T:System.Xml.XmlWriter" /> 流。</param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("match");
            Match.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("invoke");
            writer.WriteAttributeString("type", typeof(ModuleBuildStepElement).GetNoVersionTypeName());
            foreach (ModuleBuildStepElement step in Steps)
            {
                step.ObjectWriteXml(writer);
            }
            writer.WriteEndElement();
        }

    }
}
