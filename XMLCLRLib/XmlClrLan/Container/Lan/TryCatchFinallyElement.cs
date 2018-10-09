using System;
using System.Configuration;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XmlClrLan
{
    /// <summary>
    /// 实现TryCatchFinally
    /// </summary>
    /// <seealso cref="XmlClrLan.StepBasedExpressionItem{XmlClrLan.VOID}" />
    public class TryCatchFinallyElement : StepBasedExpressionItem<VOID>
    {
        /// <summary>
        /// 异常捕获委托
        /// </summary>
        /// <value>
        /// The catch.
        /// </value>
        [XmlElement(ElementName = "catch")]
        public TypeValueElement Catch { get; set; }

        [XmlElement(ElementName = "finally")]
        public SubModuleBuildElement Finally { get; set; }

        /// <summary>
        /// 实现代码逻辑
        /// </summary>
        /// <param name="scope">The scope.</param>
        public override VOID Invoke(ModuleRunScope scope)
        {
            if (Builder == null)
                throw new ConfigurationErrorsException("请先设置构建器Builder：(" + typeof(SubModuleBuildElement).FullName + ")！");

            try
            {
                InvokeBuilderInScope(Builder, scope, false);
            }
            catch (Exception err)
            {
                if (Catch != null)
                {
                    string delegateStr = Catch.Value.ToString();
                    Action<Exception> handler = delegateStr.CreateFromConfig<Action<Exception>>();
                    if (handler != null)
                        handler(err);
                }
            }
            finally
            {
                if (Finally != null)
                    InvokeBuilderInScope(Finally, scope, false);
            }
            return VOID.Empty;
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象。
        /// </summary>
        /// <param name="reader">从中对对象进行反序列化的 <see cref="T:System.Xml.XmlReader" /> 流。</param>
        public override void ReadXml(XmlReader reader)
        {
            reader.ReadToElement("Builder");
            Builder = reader.ObjectReadXml<SubModuleBuildElement>();
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 <see cref="T:System.Xml.XmlWriter" /> 流。</param>
        public override void WriteXml(XmlWriter writer)
        {
            if (Builder != null)
                Builder.ObjectWriteXml(writer);
        }

    }
}
