using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace XmlClrLan
{
    [Serializable, Description("布尔值 运行域逻辑字段条件")]
    public class BooleanConditionalItem : IConditionalItem
    {
        /// <summary>
        /// 与本项条件的逻辑运算规则
        /// </summary>
        /// <value></value>
        [XmlAttribute]
        public LogicExpression Logic { get; set; }

        /// <summary>
        /// 作用域中特定键名
        /// </summary>
        [XmlAttribute, Description("运行域键名")]
        public string Key { get; set; }

        /// <summary>
        /// 布尔表达式（优先于键值条件匹配）
        /// </summary>
        [XmlElement(ElementName = "Expression")]
        public StepBasedExpressionItem<bool> Expression { get; set; }

        /// <summary>
        /// 当前条件是否通过
        /// </summary>
        /// <param name="scope">作用域</param>
        /// <returns>
        ///   <c>true</c> if the specified scope is passed; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsPassed(ModuleRunScope scope)
        {
            if (Expression != null)
            {
                bool result = Expression.Invoke(scope);
                return result;
            }

            object boolKey = scope.GetVaraible(Key);
            if (boolKey == null)
                return false;
            else
                return Convert.ToBoolean(boolKey);
        }
    }
}
