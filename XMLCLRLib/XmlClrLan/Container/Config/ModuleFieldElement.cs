using System;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace XmlClrLan
{
    /// <summary>
    /// 模块字段绑定
    /// </summary>
    /// <seealso cref="XmlClrLan.ModuleBuildStepElement" />
    [Serializable]
    [XmlRoot(ElementName = "Binding")]
    public class ModuleFieldElement : ModuleBuildStepElement
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public TypeValueElement ValueType { get; set; }

        [XmlAttribute(AttributeName = "flags")]
        public BindingFlags Flags { get; set; }

        /// <summary>
        /// 当绑定为静态类型时全称
        /// </summary>
        [XmlAttribute(AttributeName = "staticType")]
        public string StaticType { get; set; }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象。
        /// </summary>
        /// <param name="reader">从中对对象进行反序列化的 <see cref="T:System.Xml.XmlReader" /> 流。</param>
        public override void ReadXml(XmlReader reader)
        {
            if (reader.HasAttributes)
            {
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == "name")
                    {
                        Name = reader.Value;
                    }
                    else if (reader.Name == "flags")
                    {
                        Flags = reader.Value.AsBindingFlags();
                    }
                    else if (reader.Name == "staticType")
                    {
                        StaticType = reader.Value;
                    }
                }
            }

            int entDepth = reader.Depth;
            while (reader.Read() && reader.Depth >= entDepth)
            {
                //处理开始节点
                if (reader.Name == "ValueType" && reader.NodeType == XmlNodeType.Element)
                {
                    ValueType = reader.ObjectReadXml<TypeValueElement>();
                }
            }
        }


        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 <see cref="T:System.Xml.XmlWriter" /> 流。</param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("name", Name.ToString());
            if (!string.IsNullOrEmpty(StaticType))
            {
                writer.WriteAttributeString("staticType", StaticType.ToString());
            }
            writer.WriteStartElement("ValueType");
            ValueType.WriteXml(writer);
            writer.WriteEndElement();
        }

        /// <summary>
        /// 在作用域下执行
        /// </summary>
        /// <param name="instanceType">实例类型</param>
        /// <param name="instance">The instance.</param>
        /// <param name="scope">执行代码作用区间</param>
        public override bool InvokeInScope(Type instanceType, object instance, ModuleRunScope scope = null)
        {
            if (!string.IsNullOrEmpty(StaticType))
            {
                instanceType = TypeCache.GetRuntimeType(StaticType);
                Flags |= BindingFlags.Static;
                instance = null;
            }

            FieldInfo mInfo = instanceType.GetField(Name, Flags);
            if (mInfo == null)
            {
                throw new System.Configuration.ConfigurationErrorsException("字段[" + Name + "]在类型(" + instanceType.FullName + ")中未找到，请确保系统配置（flags）正确！");
            }
            else
            {
                if ((Flags & BindingFlags.SetField) == BindingFlags.SetField)
                {
                    mInfo.SetValue(instance, ValueType.GetObjectValue());
                }

                if ((Flags & BindingFlags.GetField) == BindingFlags.GetField)
                {
                    scope.StepSwap = mInfo.GetValue(instance);
                }
            }
            return false;
        }
    }
}
