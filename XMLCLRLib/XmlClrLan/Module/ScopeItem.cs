using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace XmlClrLan
{
    public class ScopeItem
    {

    }

    [Serializable]
    [XmlRoot(ElementName = "Disposable")]
    public class DisposableScopeItem : StepBasedExpressionItem<IDisposable>, IDisposable, IXmlSerializable
    {

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


        public void Dispose()
        {

        }
    }
}
