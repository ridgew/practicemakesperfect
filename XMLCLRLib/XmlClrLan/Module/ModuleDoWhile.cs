using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace XmlClrLan
{
    [Serializable]
    public class ModuleDoWhile : ModuleBlock
    {
        [XmlElement(ElementName = "do")]
        public ModuleDo Do { get; set; }

        [XmlElement(ElementName = "while")]
        public ModuleWhile While { get; set; }

        [XmlElement(ElementName = "mode")]
        public DoWhileMode RunMode { get; set; }

        /// <summary>
        /// 根据配置执行循环逻辑
        /// </summary>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">模块类型配置错误，至少需要有while满足的条件！</exception>
        public override void InvokeInScope(ModuleRunScope scope)
        {
            if (While == null)
                throw new System.Configuration.ConfigurationErrorsException("模块类型(" + typeof(ModuleDoWhile).FullName + ")配置错误，至少需要有while满足的条件！");

            if (RunMode == DoWhileMode.While)
            {
                while (While.Match.CanRunInScope(scope))
                {
                    While.InvokeInScope(scope);
                }
            }
            else if (RunMode == DoWhileMode.DoWhile)
            {
                do
                {
                    Do.InvokeInScope(scope);
                }
                while (While.Match.CanRunInScope(scope));
            }
            else if (RunMode == DoWhileMode.WhileDo)
            {
                while (While.Match.CanRunInScope(scope))
                {
                    Do.InvokeInScope(scope);
                }
            }
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 <see cref="T:System.Xml.XmlWriter" /> 流。</param>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象。
        /// </summary>
        /// <param name="reader">从中对对象进行反序列化的 <see cref="T:System.Xml.XmlReader" /> 流。</param>

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
        }
    }

    [Serializable]
    public enum DoWhileMode
    {
        While = 0,
        DoWhile = 1,
        WhileDo = 2
    }

    [Serializable]
    public class ModuleDo : ModuleBlock
    {

    }

    [Serializable]
    public class ModuleWhile : ModuleBlock
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
            base.ReadXml(reader);
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 <see cref="T:System.Xml.XmlWriter" /> 流。</param>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
        }
    }

    [Serializable]
    public class ModuleIterator : ModuleBlock
    {
        //IEnumerable
    }

    [Serializable]
    public class IncreaseModuleIterator : ModuleIterator
    {

    }

    [Serializable]
    public class DescreaseModuleIterator : ModuleIterator
    {

    }

}
