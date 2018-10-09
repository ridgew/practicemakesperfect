using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XmlClrLan
{
    /// <summary>
    /// 判断某个实例是否是特定数据类型
    /// </summary>
    /// <seealso cref="XmlClrLan.IConditionalItem" />
    public class InstanceIsTypeConditionItem : IConditionalItem
    {
        [XmlAttribute]
        public LogicExpression Logic { get; set; }


        [XmlAttribute(AttributeName = "key")]
        public string ScopeInstanceKey { get; set; }


        [XmlAttribute(AttributeName = "cmpType")]
        public string CompareType { get; set; }

        /// <summary>
        /// 当前条件是否通过
        /// </summary>
        /// <param name="scope">作用域</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool IsPassed(ModuleRunScope scope)
        {
            object cmp = scope.GetVaraible(ScopeInstanceKey);
            if (cmp == null)
                throw new System.Configuration.ConfigurationErrorsException("作用域变量[" + ScopeInstanceKey + "]未找到，请确保系统配置正确！");
            Type cmpType = TypeCache.GetRuntimeType(CompareType);
            return cmp.GetType().Equals(cmpType);
        }
    }

}
