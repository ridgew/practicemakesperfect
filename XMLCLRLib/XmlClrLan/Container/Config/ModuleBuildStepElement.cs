using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XmlClrLan
{
    /// <summary>
    /// 模块创建步骤节点（抽象类,实现类需要实现XML序列化）
    /// </summary>
    [Serializable]
    public abstract class ModuleBuildStepElement : IXmlSerializable
    {
        /// <summary>
        /// 所属模块（容器)，只读
        /// </summary>
        [XmlIgnore]
        public SubModuleBuildElement Module { get; internal set; }

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
        /// 在作用域下执行
        /// </summary>
        /// <param name="instanceType">实例类型</param>
        /// <param name="instance">The instance.</param>
        /// <param name="scope">执行代码作用区间</param>
        /// <returns>是否中途中止进行</returns>
        public abstract bool InvokeInScope(Type instanceType, object instance, ModuleRunScope scope = null);


        /// <summary>
        /// 从对象的 XML 表示形式生成该对象。
        /// </summary>
        /// <param name="reader">从中对对象进行反序列化的 <see cref="T:System.Xml.XmlReader" /> 流。</param>
        public abstract void ReadXml(XmlReader reader);


        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 <see cref="T:System.Xml.XmlWriter" /> 流。</param>
        public abstract void WriteXml(XmlWriter writer);
    }
}
