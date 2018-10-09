using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace XmlClrLan
{
    [Serializable]
    [XmlRoot(ElementName = "arg")]
    public class ModuleConstructorElement : ModuleBuildStepElement
    {
        [XmlElement(ElementName = "param")]
        public TypeValueElement[] Arguments { get; set; }

        public object[] GetArguments(ModuleRunScope scope = null)
        {
            List<object> args = new List<object>();
            if (Arguments != null && Arguments.Any())
            {
                foreach (var parm in Arguments)
                {
                    args.Add(parm.GetObjectValue(scope));
                }
            }
            return args.ToArray();
        }

        public Type[] GetArgumentTypes()
        {
            List<Type> args = new List<Type>();
            if (Arguments != null && Arguments.Any())
            {
                foreach (var parm in Arguments)
                {
                    args.Add(parm.GetClrType());
                }
            }
            return args.ToArray();
        }

        /// <summary>
        /// 在作用域下执行
        /// </summary>
        /// <param name="instanceType">实例类型</param>
        /// <param name="instance">The instance.</param>
        /// <param name="scope">执行代码作用区间</param>
        public override bool InvokeInScope(Type instanceType, object instance, ModuleRunScope scope = null)
        {
            if (scope != null)
            {
                scope.StepSwap = Activator.CreateInstance(instanceType, GetArguments());
            }
            return false;
        }


        /// <summary>
        /// 从对象的 XML 表示形式生成该对象。
        /// </summary>
        /// <param name="reader">从中对对象进行反序列化的 <see cref="T:System.Xml.XmlReader" /> 流。</param>
        public override void ReadXml(XmlReader reader)
        {
            List<TypeValueElement> typeList = new List<TypeValueElement>();
            int entDepth = reader.Depth;
            while (reader.Read() && reader.Depth >= entDepth)
            {
                //处理开始节点
                while (reader.NodeType == XmlNodeType.Element)
                {
                    TypeValueElement ele = reader.ObjectReadXml<TypeValueElement>();
                    typeList.Add(ele);
                }
            }
            Arguments = typeList.ToArray();
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 <see cref="T:System.Xml.XmlWriter" /> 流。</param>
        public override void WriteXml(XmlWriter writer)
        {
            if (Arguments != null)
            {
                foreach (var arg in Arguments)
                    arg.ObjectWriteXml(writer);
            }
        }

    }
}
