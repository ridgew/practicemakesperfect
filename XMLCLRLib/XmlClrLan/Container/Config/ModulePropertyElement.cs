using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Diagnostics;
using System.Xml.Serialization;

namespace XmlClrLan
{
    [Serializable]
    [XmlRoot(ElementName = "Set")]
    [DebuggerDisplay("属性设置 Name:{Name}, Value:{Value}")]
    public class ModulePropertyElement : ModuleBuildStepElement
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }

        [XmlAttribute(AttributeName = "flags")]
        public BindingFlags Flags { get; set; }

        /// <summary>
        /// 在作用域下执行
        /// </summary>
        /// <param name="instanceType">实例类型</param>
        /// <param name="instance">The instance.</param>
        /// <param name="scope">执行代码作用区间</param>
        public override bool InvokeInScope(Type instanceType, object instance, ModuleRunScope scope = null)
        {
            PropertyInfo pInfo = instanceType.GetProperty(Name, BindingFlags.Public | BindingFlags.Instance);
            if (pInfo == null)
            {
                throw new System.Configuration.ConfigurationErrorsException("属性[" + Name + "]在类型(" + instanceType.FullName + ")中未找到，请确保系统配置正确！");
            }
            else
            {
                object targetVal = Value;
                //if (string.IsNullOrEmpty(Value))
                //{
                //}
                int idx = Value.IndexOf('$');
                if (idx != -1)
                {
                    if (Value[idx + 1] != '$')
                    {
                        string str = "\\$([a-zA-z_][a-zA-z_0-9]*)";
                        string varName = Regex.Match(Value, str).Groups[1].Value;
                        targetVal = scope.GetVaraible(varName);
                    }
                }

                pInfo.SetValue(instance, targetVal, null);
            }
            return false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleBuildStepElement" /> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
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
                    else if (reader.Name == "value")
                    {
                        Value = reader.Value;
                    }
                    else if (reader.Name == "flags")
                    {
                        Flags = reader.Value.AsBindingFlags();
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleBuildStepElement" /> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("name", Name.ToString());
            writer.WriteAttributeString("value", Value.ToString());
            writer.WriteAttributeString("flags", Flags.ToString());
        }

    }
}
