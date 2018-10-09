using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XmlClrLan
{
    [Serializable, Description("字符串比较 运行域字段条件")]
    public class StringConditionItem : IConditionalItem, IXmlSerializable
    {
        /// <summary>
        /// 与本项条件的逻辑运算规则
        /// </summary>
        /// <value></value>
        [XmlAttribute(AttributeName = "logic")]
        public LogicExpression Logic { get; set; }

        /// <summary>
        /// 作用域中特定键名(布尔值)
        /// </summary>
        [XmlAttribute(AttributeName = "key"), Description("运行域键名")]
        public string Key { get; set; }

        /// <summary>
        /// 字符表达式
        /// </summary>
        [XmlElement(ElementName = "left")]
        public IExpressionItem<string> ExpressionLeft { get; set; }

        /// <summary>
        /// 右侧表达式
        /// </summary>
        [XmlElement(ElementName = "right")]
        public IExpressionItem<string> ExpressionRight { get; set; }

        /// <summary>
        /// 比较模式
        /// </summary>
        [XmlAttribute(AttributeName = "mode"), Description("比较模式，是否相等")]
        public StringComparison ComparisonMode { get; set; }

        /// <summary>
        /// 布尔比较委托(原型：Func&lt;object, object, bool&gt;）
        /// </summary>
        [XmlAttribute(AttributeName = "boolDelegate")]
        public string BooleanCompareDelegate { get; set; }

        /// <summary>
        /// 此方法是保留方法，请不要使用。
        /// 在实现 <see langword="IXmlSerializable" /> 接口时，应从此方法返回 <see langword="null" />（在 Visual Basic 中为 <see langword="Nothing" />），如果需要指定自定义架构，应向该类应用 <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" />。
        /// </summary>
        /// <returns>
        /// 一个 <see cref="T:System.Xml.Schema.XmlSchema" />，描述由 <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> 方法生成并由 <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> 方法使用的对象的 XML 表示形式。
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// 当前条件是否通过
        /// </summary>
        /// <param name="scope">作用域</param>
        /// <returns>
        ///   <c>true</c> if the specified scope is passed; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsPassed(ModuleRunScope scope)
        {
            if (ExpressionLeft != null && ExpressionRight != null)
            {
                string result = ExpressionLeft.Invoke(scope);
                string resultRight = ExpressionRight.Invoke(scope);

                if (!string.IsNullOrEmpty(BooleanCompareDelegate))
                    return ConditionalElement.GetBooleanDelegate(BooleanCompareDelegate)(result, resultRight);
                else
                    return string.Equals(result, resultRight, ComparisonMode);
            }

            object boolKey = scope.GetVaraible(Key);
            if (boolKey == null)
                return false;
            else
                return Convert.ToBoolean(boolKey);
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.HasAttributes)
            {
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == "key")
                    {
                        Key = reader.Value;
                    }
                    else if (reader.Name == "mode")
                    {
                        ComparisonMode = (StringComparison)Enum.Parse(typeof(StringComparison), reader.Value);
                    }
                    else if (reader.Name == "logic")
                    {
                        Logic = (LogicExpression)Enum.Parse(typeof(LogicExpression), reader.Value);
                    }
                    else if (reader.Name == "boolDelegate")
                    {
                        BooleanCompareDelegate = reader.Value;
                    }
                }
            }

            int entDepth = reader.Depth;
            while (reader.Read() && reader.Depth >= entDepth)
            {
                //处理开始节点
                while (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "left")
                    {
                        Type targetType = TypeCache.GetRuntimeType(reader.GetAttribute("type"));
                        reader.Read();
                        ExpressionLeft = reader.ObjectReadXmlType(targetType) as IExpressionItem<string>;
                    }
                    if (reader.Name == "right")
                    {
                        Type targetTypeRight = TypeCache.GetRuntimeType(reader.GetAttribute("type"));
                        reader.Read();
                        ExpressionRight = reader.ObjectReadXmlType(targetTypeRight) as IExpressionItem<string>;
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(Key))
                writer.WriteAttributeString("key", Key);
            if (!string.IsNullOrEmpty(BooleanCompareDelegate))
                writer.WriteAttributeString("boolDelegate", BooleanCompareDelegate);

            writer.WriteAttributeString("logic", Logic.ToString());
            writer.WriteAttributeString("mode", ComparisonMode.ToString());

            if (ExpressionLeft != null)
            {
                writer.WriteStartElement("left");
                writer.WriteAttributeString("type", ExpressionLeft.GetType().ToSimpleType());
                ExpressionLeft.ObjectWriteXml(writer);
                writer.WriteEndElement();
            }

            if (ExpressionRight != null)
            {
                writer.WriteStartElement("right");
                writer.WriteAttributeString("type", ExpressionRight.GetType().ToSimpleType());
                ExpressionRight.ObjectWriteXml(writer);
                writer.WriteEndElement();
            }

        }
    }
}
