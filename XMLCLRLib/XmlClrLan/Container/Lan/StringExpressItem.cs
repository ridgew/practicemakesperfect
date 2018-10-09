using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XmlClrLan
{
    [Serializable]
    [XmlRoot(ElementName = "string")]
    public class StringExpressItem : IExpressionItem<string>, IXmlSerializable
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }

        public string Invoke(ModuleRunScope scope)
        {
            if (Value.IndexOf('$') != -1)
            {
                if (Value == "$StepSwap")
                    return scope.StepSwap.ToString();
            }

            return Value;
        }

        /// <summary>
        /// 此方法是保留方法，请不要使用。
        /// 在实现 <see langword="IXmlSerializable" /> 接口时，应从此方法返回 <see langword="null" />（在 Visual Basic 中为 <see langword="Nothing" />），如果需要指定自定义架构，应向该类应用 <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" />。
        /// </summary>
        /// <returns>
        /// 一个 <see cref="T:System.Xml.Schema.XmlSchema" />，描述由 <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> 方法生成并由 <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> 方法使用的对象的 XML 表示形式。
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象。
        /// </summary>
        /// <param name="reader">从中对对象进行反序列化的 <see cref="T:System.Xml.XmlReader" /> 流。</param>
        public void ReadXml(XmlReader reader)
        {
            if (reader.HasAttributes)
            {
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == "value")
                    {
                        Value = reader.Value;
                    }
                }
            }
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 <see cref="T:System.Xml.XmlWriter" /> 流。</param>
        public void WriteXml(XmlWriter writer)
        {
            if (Value != null)
                writer.WriteAttributeString("value", Value);
        }

        /// <summary>
        /// 显式转换为字符<see cref="System.String"/>为 <see cref="StringExpressItem"/>.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator StringExpressItem(string d)
        {
            return new StringExpressItem { Value = d };
        }


        /// <summary>
        /// 隐式转换 <see cref="StringExpressItem"/> 为 <see cref="System.String"/>.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator string(StringExpressItem d)
        {
            return d.Value;
        }

    }
}
