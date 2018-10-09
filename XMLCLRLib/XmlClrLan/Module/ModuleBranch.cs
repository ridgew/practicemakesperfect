using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace XmlClrLan
{
    /// <summary>
    /// 条件分支配置
    /// </summary>
    [XmlRoot(ElementName = "Case")]
    [Serializable]
    public class ModuleBranch : ModuleBuildStepElement
    {
        /// <summary>
        /// 运行条件配置(when)
        /// </summary>
        [XmlElement(ElementName = "when")]
        public ModuleWhen[] Conditions { get; set; }

        /// <summary>
        /// 非满足条件时执行
        /// </summary>
        [XmlElement(ElementName = "else")]
        public SubModuleBuildElement Else { get; set; }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象。
        /// </summary>
        /// <param name="reader">从中对对象进行反序列化的 <see cref="T:System.Xml.XmlReader" /> 流。</param>
        public override void ReadXml(XmlReader reader)
        {
            int entDepth = reader.Depth;

            reader.ReadToElement("when");
            List<ModuleWhen> wList = new List<ModuleWhen>();
            while (reader.Name == "when" || reader.NodeType == XmlNodeType.Comment)
            {
                if (reader.NodeType == XmlNodeType.Comment)
                {
                    reader.ReadToElement("when");
                    continue;
                }

                ModuleWhen myWhen = new ModuleWhen();
                myWhen.ReadXml(reader);
                wList.Add(myWhen);

                if (reader.Name == "when" && reader.NodeType == XmlNodeType.EndElement)
                    break;
            }
            Conditions = wList.ToArray();

            while (reader.NodeType != XmlNodeType.Element && reader.Depth > entDepth)
                reader.Read();

            if (reader.NodeType == XmlNodeType.Element && reader.Name == "else")
            {
                SubModuleBuildElement myElse = new SubModuleBuildElement();
                myElse.ReadXml(reader);
                Else = myElse;
            }

        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 <see cref="T:System.Xml.XmlWriter" /> 流。</param>
        public override void WriteXml(XmlWriter writer)
        {
            foreach (ModuleWhen when in Conditions)
            {
                when.ObjectWriteXml(writer);
            }

            //Else
            if (Else != null)
            {
                writer.WriteStartElement("else");
                Else.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// 在作用域下执行
        /// </summary>
        /// <param name="instanceType">实例类型</param>
        /// <param name="instance">The instance.</param>
        /// <param name="scope">执行代码作用区间</param>
        /// <returns>是否中途中止进行</returns>
        public override bool InvokeInScope(Type instanceType, object instance, ModuleRunScope scope = null)
        {
            if (Conditions != null)
            {
                ModuleWhen whenMatched = null;
                for (int i = 0, j = Conditions.Length; i < j; i++)
                {
                    if (Conditions[i].Match.CanRunInScope(scope))
                    {
                        whenMatched = Conditions[i];
                        break;
                    }
                }

                if (whenMatched != null)
                {
                    return whenMatched.InvokeInScope(instanceType, instance, scope);
                }
                else
                {
                    if (Else != null)
                        return Else.InvokeInScope(instanceType, instance, scope);
                }
            }
            return false;
        }

    }
}
