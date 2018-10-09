using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace XmlClrLan
{
    /// <summary>
    /// 通过执行步骤求得的值类型
    /// </summary>
    /// <seealso cref="XmlClrLan.TypeValueElement" />
    [Serializable]
    [XmlRoot(ElementName = "IndirectTypeValue")]
    public class IndirectTypeValueElement : TypeValueElement
    {

        protected List<ModuleBuildStepElement> _innerSteps = new List<ModuleBuildStepElement>();

        /// <summary>
        /// 构建步骤
        /// </summary>
        [XmlIgnore]
        public ModuleBuildStepElement[] Steps
        {
            get { return _innerSteps.ToArray(); }
        }

        /// <summary>
        /// 添加构建步骤
        /// </summary>
        /// <typeparam name="TStep">构建基础类型</typeparam>
        /// <param name="step">具体的实现</param>
        public void AddStep<TStep>(TStep step)
            where TStep : ModuleBuildStepElement
        {
            _innerSteps.Add(step);
        }

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
                    if (reader.Name == "type")
                    {
                        Type = reader.Value;
                    }
                    else if (reader.Name == "value")
                    {
                        Value = reader.Value;
                    }
                    else if (reader.Name == "staticField")
                    {
                        StaticField = Convert.ToBoolean(reader.Value);
                    }
                }
            }

            int entDepth = reader.Depth;
            while (reader.Read() && reader.Depth >= entDepth)
            {
                //处理开始节点
                while (reader.NodeType == XmlNodeType.Element)
                {
                    _innerSteps.ReadXmlEx<ModuleBuildStepElement>(reader);
                }
            }

        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 <see cref="T:System.Xml.XmlWriter" /> 流。</param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("type", Type);
            writer.WriteAttributeString("value", Value.ToString());
            if (StaticField)
                writer.WriteAttributeString("staticField", StaticField.ToString());

            foreach (ModuleBuildStepElement step in _innerSteps)
            {
                step.ObjectWriteXml(writer);
            }
        }

        /// <summary>
        /// 获取运行域内的值
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public override object GetObjectValue(ModuleRunScope scope = null)
        {
            Type targetType = TypeCache.GetRuntimeType(Type);
            foreach (var step in Steps)
            {
                step.InvokeInScope(targetType, null, scope);
            }
            return scope.StepSwap;
        }
    }
}
