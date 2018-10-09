using System;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace XmlClrLan
{
    [Serializable]
    [XmlRoot(ElementName = "Call")]
    public class ModuleMethodCallElement : ModuleBuildStepElement
    {
        public ModuleMethodCallElement()
        {

        }

        public ModuleMethodCallElement(string name, TypeValueElement[] args)
        {
            MethodName = name;
            Parameters = new ModuleConstructorElement
            {
                Arguments = args
            };
        }

        [XmlAttribute(AttributeName = "fun")]
        public string MethodName { get; set; }

        [XmlElement(ElementName = "arg")]
        public ModuleConstructorElement Parameters { get; set; }

        /// <summary>
        /// 当绑定为静态类型时全称
        /// </summary>
        [XmlAttribute(AttributeName = "staticType")]
        public string StaticType { get; set; }

        /// <summary>
        /// 在作用域下执行
        /// </summary>
        /// <param name="instanceType">实例类型</param>
        /// <param name="instance">The instance.</param>
        /// <param name="scope">执行代码作用区间</param>
        public override bool InvokeInScope(Type instanceType, object instance, ModuleRunScope scope = null)
        {
            if (!string.IsNullOrEmpty(StaticType))
            {
                instanceType = TypeCache.GetRuntimeType(StaticType);
                instance = null;
            }

            Type[] argTypes = new Type[0];
            if (Parameters != null)
                argTypes = Parameters.GetArgumentTypes();

            MethodInfo fun = instanceType.GetMethod(MethodName, argTypes);
            if (fun == null)
            {
                throw new System.Configuration.ConfigurationErrorsException("方法[" + MethodName + "]在类型(" + instanceType.FullName + ")中未找到，请确保系统配置正确！");
            }
            else
            {
                object[] args = (Parameters == null) ? null : Parameters.GetArguments(scope);
                object ret = fun.Invoke(instance, args);
                scope.StepSwap = ret;
            }
            return false;
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
                    if (reader.Name == "fun")
                    {
                        MethodName = reader.Value;
                    }
                    else if (reader.Name == "staticType")
                    {
                        StaticType = reader.Value;
                    }
                }
            }

            int entDepth = reader.Depth;
            while (reader.Read() && reader.Depth >= entDepth)
            {
                Parameters = reader.ObjectReadXml<ModuleConstructorElement>();
            }
        }


        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 <see cref="T:System.Xml.XmlWriter" /> 流。</param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("fun", MethodName);
            if (!string.IsNullOrEmpty(StaticType))
            {
                writer.WriteAttributeString("staticType", StaticType.ToString());
            }
            if (Parameters != null)
                Parameters.ObjectWriteXml(writer);
        }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (base.Module != null)
                return string.Format("Type:{0}, MethodName:{1}", base.Module.Type, MethodName);
            return string.Format("Type:{0}, MethodName:{1}", typeof(ModuleMethodCallElement).FullName, MethodName);
        }
    }
}
