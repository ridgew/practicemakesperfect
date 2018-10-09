using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XmlClrLan
{
    /// <summary>
    /// 条件允许节点
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "when")]
    public class ConditionalElement : IXmlSerializable
    {
        List<IConditionalItem> condItemList = new List<IConditionalItem>();

        /// <summary>
        /// 此方法是保留方法，请不要使用。在实现 IXmlSerializable 接口时，应从此方法返回 null（在 Visual Basic 中为 Nothing），如果需要指定自定义架构，应向该类应用 <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/>。
        /// </summary>
        /// <returns>
        /// 	<see cref="T:System.Xml.Schema.XmlSchema"/>，描述由 <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> 方法产生并由 <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> 方法使用的对象的 XML 表示形式。
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 <see cref="T:System.Xml.XmlReader"/> 流。</param>
        public void ReadXml(XmlReader reader)
        {
            int entDepth = reader.Depth;
            string oldName = reader.Name;

            while (reader.Read() && reader.Depth >= entDepth)
            {
                //处理开始节点
                while (reader.NodeType == XmlNodeType.Element)
                {
                    condItemList.ReadXmlEx<IConditionalItem>(reader);
                }

                if (reader.NodeType == XmlNodeType.EndElement)
                    reader.ReadEndElement();

                if (reader.Depth == entDepth && reader.Name == oldName)
                    break;
            }
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 <see cref="T:System.Xml.XmlWriter"/> 流。</param>
        public void WriteXml(XmlWriter writer)
        {
            condItemList.WriteXmlEx<IConditionalItem>(writer);
        }


        /// <summary>
        /// 添加条件判断项
        /// </summary>
        /// <param name="item">新的条件判断项</param>
        public void AddItem(IConditionalItem item)
        {
            condItemList.Add(item);
        }

        /// <summary>
        /// 清除所有配置条件
        /// </summary>
        public void Clear()
        {
            condItemList.Clear();
        }

        /// <summary>
        /// 获取所有的条件配置项
        /// </summary>
        public IConditionalItem[] GetAllConditionals()
        {
            return condItemList.ToArray();
        }

        /// <summary>
        /// 辅助函数，从字符串委托表示法还原(原型：Func&lt;object, object, bool&gt;）
        /// </summary>
        /// <param name="boolDelegateStr">布尔委托原型的字符串表示</param>
        /// <returns></returns>
        public static Func<object, object, bool> GetBooleanDelegate(string boolDelegateStr)
        {
            if (boolDelegateStr.Contains("::"))
            {
                return boolDelegateStr.CreateFromConfig<Func<object, object, bool>>();
            }
            else
            {
                TypeValueElement tvElement = new TypeValueElement
                {
                    Type = typeof(Func<object, object, bool>).FullName,
                    StaticField = true,
                    Value = boolDelegateStr
                };
                return tvElement.GetObjectValue() as Func<object, object, bool>;
            }
        }

        #region 常量配置
        /// <summary>
        /// 字符相等
        /// </summary>
        public static readonly Func<object, object, bool> StringEqual = (a, b) =>
        {
            return a.ToString().Equals(b.ToString());
        };

        /// <summary>
        /// 字符不等
        /// </summary>
        public static readonly Func<object, object, bool> StringNotEqual = (a, b) =>
        {
            return !a.ToString().Equals(b.ToString());
        };

        /// <summary>
        /// 字符包含
        /// </summary>
        public static readonly Func<object, object, bool> StringContains = (a, b) =>
        {
            return a.ToString().Contains(b.ToString());
        };

        /// <summary>
        /// 字符不包含
        /// </summary>
        public static readonly Func<object, object, bool> StringNotContains = (a, b) =>
        {
            return !a.ToString().Contains(b.ToString());
        };
        #endregion

        /// <summary>
        /// 判断是否能在当前作用域下运行
        /// </summary>
        /// <param name="scope">作用域</param>
        /// <returns></returns>
        public bool CanRunInScope(ModuleRunScope scope)
        {
            bool result = false;
            foreach (IConditionalItem item in condItemList)
            {
                switch (item.Logic)
                {
                    case LogicExpression.None:
                        result = item.IsPassed(scope);
                        break;
                    case LogicExpression.AND:
                        result = result && item.IsPassed(scope);
                        break;
                    case LogicExpression.OR:
                        result = result || item.IsPassed(scope);
                        break;
                    case LogicExpression.AndNot:
                        result = result && !item.IsPassed(scope);
                        break;
                    case LogicExpression.OrNot:
                        result = result || !item.IsPassed(scope);
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

    }

    /// <summary>
    /// 条件配置项
    /// </summary>
    public interface IConditionalItem
    {
        /// <summary>
        /// 与本项条件的逻辑运算规则
        /// </summary>
        [Description("逻辑运算规则")]
        LogicExpression Logic { get; set; }

        /// <summary>
        /// 当前条件是否通过
        /// </summary>
        /// <param name="scope">作用域</param>
        bool IsPassed(ModuleRunScope scope);

    }

    /// <summary>
    /// 逻辑运算规则
    /// </summary>
    public enum LogicExpression
    {
        /// <summary>
        /// 没有设置
        /// </summary>
        [Description("无")]
        None = 0,

        /// <summary>
        /// 并且
        /// </summary>
        [Description("并且")]
        AND,

        /// <summary>
        /// 或者
        /// </summary>
        [Description("或者")]
        OR,

        /// <summary>
        /// 并且不
        /// </summary>
        [Description("并且不")]
        AndNot,

        /// <summary>
        /// 或者不
        /// </summary>
        [Description("或者不")]
        OrNot

    }

}
