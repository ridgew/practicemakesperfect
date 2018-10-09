using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace XmlClrLan
{
    [Serializable]
    [XmlRoot(ElementName = "InstanceMethod")]
    public class InstanceMethodGetElement : ModuleBuildStepElement
    {
        /// <summary>
        /// 函数原型定义模型
        /// </summary>
        [XmlAttribute(AttributeName = "typedef")]
        public string MethodDelegatePattern { get; set; }

        /// <summary>
        /// 函数名称
        /// </summary>
        [XmlAttribute(AttributeName = "fun")]
        public string MethodName { get; set; }

        /// <summary>
        /// 实例来源
        /// </summary>
        [XmlAttribute(AttributeName = "origin")]
        public InstanceOrigin OriginType { get; set; }

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
                    if (reader.Name == "typedef")
                    {
                        MethodDelegatePattern = reader.Value;
                    }
                    else if (reader.Name == "fun")
                    {
                        MethodName = reader.Value;
                    }
                    else if (reader.Name == "origin")
                    {
                        OriginType = (InstanceOrigin)Enum.Parse(typeof(InstanceOrigin), reader.Value);
                    }
                }
            }
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 <see cref="T:System.Xml.XmlWriter" /> 流。</param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("typedef", MethodDelegatePattern);
            writer.WriteAttributeString("fun", MethodName);
            writer.WriteAttributeString("origin", OriginType.ToString());
        }

        /// <summary>
        /// 在作用域下执行
        /// </summary>
        /// <param name="instanceType">实例类型</param>
        /// <param name="instance">The instance.</param>
        /// <param name="scope">执行代码作用区间</param>
        /// <returns>
        /// 是否中途中止进行
        /// </returns>
        public override bool InvokeInScope(Type instanceType, object instance, ModuleRunScope scope = null)
        {
            object targetInstance = null;
            switch (OriginType)
            {
                case InstanceOrigin.Context:
                    targetInstance = scope.StepSwap;
                    break;
                case InstanceOrigin.Arguments:
                    targetInstance = instance;
                    break;
                case InstanceOrigin.BuildInType:
                    targetInstance = Activator.CreateInstance(instanceType);
                    break;
                default:
                    break;
            }

            instanceType = targetInstance.GetType();
            Type delegateType = TypeCache.GetRuntimeType(MethodDelegatePattern);

            MethodInfo mInfo = instanceType.GetMethod(MethodName, BindingFlags.Instance | BindingFlags.Public);
            if (mInfo == null)
                throw new System.Configuration.ConfigurationErrorsException("函数[" + MethodName + "]在类型(" + instanceType.FullName + ")中未找到，请确保系统配置正确！");

            Delegate target = Delegate.CreateDelegate(delegateType, targetInstance, mInfo, true); //委托绑定到实例方法

            scope.StepSwap = target;

            return false;
        }

    }

    [Serializable]
    public enum InstanceOrigin
    {
        /// <summary>
        /// 来自上下文
        /// </summary>
        Context = 0,
        /// <summary>
        /// 调用请求参数
        /// </summary>
        Arguments = 1,
        /// <summary>
        /// 构建类型
        /// </summary>
        BuildInType = 2
    }

}
