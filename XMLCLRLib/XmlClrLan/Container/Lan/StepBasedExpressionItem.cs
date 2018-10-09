using System;
using System.Configuration;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XmlClrLan
{
    [Serializable]
    public class StepBasedExpressionItem<Target> : IExpressionItem<Target>, IXmlSerializable
    {
        [XmlElement(ElementName = "Builder")]
        public SubModuleBuildElement Builder { get; set; }


        public virtual Target Invoke(ModuleRunScope scope)
        {
            if (Builder == null)
                throw new ConfigurationErrorsException("请先设置构建器Builder：(" + typeof(SubModuleBuildElement).FullName + ")！");

            object val = InvokeBuilderInScope(Builder, scope, false);
            if (Builder.Target == BuildTarget.Instance)
                return (Target)val;
            else
                return (Target)val;
        }

        /// <summary>
        /// Invokes the builder in scope.
        /// </summary>
        /// <param name="sub">The sub.</param>
        /// <param name="scope">执行作用域</param>
        /// <param name="ignoreException">是否忽略执行异常</param>
        /// <returns></returns>
        protected static object InvokeBuilderInScope(SubModuleBuildElement sub, ModuleRunScope scope, bool ignoreException = false)
        {
            Type taregetType = TypeCache.GetRuntimeType(sub.Type);
            ModuleConstructorElement constr = sub.Steps.FirstOrDefault(s => s.GetType().Equals(typeof(ModuleConstructorElement))) as ModuleConstructorElement;
            object instance = null;

            #region 构建对象
            if (constr == null)
            {
                if (sub.BuildInstance)
                {
                    //if (taregetType.IsValueType)
                    if (taregetType == typeof(string))
                        instance = string.Empty;
                    else
                        instance = Activator.CreateInstance(taregetType);
                }
                else
                {
                    instance = default(Target);
                }
            }
            else
            {
                if (scope != null)
                    scope.AddStackFrame(constr.ToString());
                constr.InvokeInScope(taregetType, null, scope);
                instance = scope.StepSwap;
            }

            if (sub.BuildInstance && instance == null)
            {
                throw new ConfigurationErrorsException("创建实例：(" + taregetType.FullName + ")失败，请确保系统配置正确！");
            }

            #endregion

            try
            {
                foreach (var step in sub.Steps)
                {
                    if (scope != null)
                        scope.AddStackFrame(step.ToString());
                    step.InvokeInScope(taregetType, instance, scope);
                }
            }
            catch (Exception ivkError)
            {
                scope.LastError = ivkError;
                if (!ignoreException)
                    throw ivkError;
            }

            if (sub.Target == BuildTarget.Instance)
                return instance;
            else
                return scope.StepSwap;
        }

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
        /// 从对象的 XML 表示形式生成该对象。
        /// </summary>
        /// <param name="reader">从中对对象进行反序列化的 <see cref="T:System.Xml.XmlReader" /> 流。</param>
        public virtual void ReadXml(XmlReader reader)
        {
            reader.ReadToElement("Builder");
            Builder = reader.ObjectReadXml<SubModuleBuildElement>();
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 <see cref="T:System.Xml.XmlWriter" /> 流。</param>
        public virtual void WriteXml(XmlWriter writer)
        {
            if (Builder != null)
                Builder.ObjectWriteXml(writer);
        }

    }
}
