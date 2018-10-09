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
    [XmlRoot(ElementName = "AnyMatch")]
    public class ModuleAnyMatch : ModuleBlock
    {
        /// <summary>
        /// 运行条件配置(match)
        /// </summary>
        [XmlElement(ElementName = "match")]
        public ConditionalElement Match { get; set; }

        [XmlElement(ElementName = "enumerator")]
        public string ScopeIEnumerableInstance { get; set; }

        /// <summary>
        /// 在作用域下单独执行
        /// </summary>
        /// <param name="scope"></param>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">系统配置错误</exception>
        public override void InvokeInScope(ModuleRunScope scope)
        {
            if (BuildInstance)
            {
                Type taregetType = TypeCache.GetRuntimeType(Type);
                object instance = InvokeInScopeContructor(scope);
                InvokeInScope(taregetType, instance, scope);
            }
            else
            {
                object instance = scope.StepSwap;
                Type taregetType = instance != null ? instance.GetType() : TypeCache.GetRuntimeType(Type);
                InvokeInScope(taregetType, instance, scope);
            }
        }

        /// <summary>
        /// 在作用域下执行
        /// </summary>
        /// <param name="instanceType">实例类型</param>
        /// <param name="instance">The instance.</param>
        /// <param name="scope">执行代码作用区间</param>
        /// <returns>是否中途中止进行</returns>
        public override bool InvokeInScope(Type instanceType, object instance, ModuleRunScope scope = null)
        {
            object enumeratorObj = scope.GetVaraible(ScopeIEnumerableInstance);
            if (enumeratorObj == null)
                throw new System.Configuration.ConfigurationErrorsException("作用域变量[" + ScopeIEnumerableInstance + "]未找到，请确保系统配置正确！");

            Type objType = enumeratorObj.GetType();
            System.Collections.IEnumerator enumerator = null;
            if (objType.IsArray)
            {
                Array arr = enumeratorObj as Array;
                enumerator = arr.GetEnumerator();
            }
            else
            {
                MethodInfo mInfo = objType.GetMethod("GetEnumerator", BindingFlags.InvokeMethod | BindingFlags.Instance);
                if (mInfo == null)
                    throw new System.Configuration.ConfigurationErrorsException("作用域变量[" + ScopeIEnumerableInstance + "]没有实现GetEnumerator()的方法，不是一个可枚举的实例类型，请确保系统配置正确！");
                enumerator = mInfo.Invoke(enumeratorObj, null) as System.Collections.IEnumerator;
            }

            if (enumerator == null)
            {
                throw new System.Configuration.ConfigurationErrorsException("系统传参错误，作用域变量[" + ScopeIEnumerableInstance + "]不是一个可枚举的实例类型，请确保系统配置正确！");
            }

            bool stopInvoke = false;
            using (ModuleRunScope nScope = new ModuleRunScope(scope))
            {
                while (enumerator.MoveNext())
                {
                    nScope.StepSwap = enumerator.Current;
                    if (Match.CanRunInScope(nScope))
                    {
                        bool exit = InvokeStepsInScope(instanceType, instance, nScope, base.Steps);
                        if (exit)
                        {
                            stopInvoke = true;
                            break;
                        }
                    }
                }
            }
            return stopInvoke;
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
                    if (reader.Name == "moduleId")
                    {
                        ModuleId = reader.Value;
                    }
                    else if (reader.Name == "type")
                    {
                        Type = reader.Value;
                    }
                    else if (reader.Name == "isInstance")
                    {
                        BuildInstance = Convert.ToBoolean(reader.Value);
                    }
                    else if (reader.Name == "target")
                    {
                        Target = (BuildTarget)Enum.Parse(typeof(BuildTarget), reader.Value);
                    }
                    else if (reader.Name == "enumerator")
                    {
                        ScopeIEnumerableInstance = reader.Value;
                    }
                }
            }

            int entDepth = reader.Depth;
            while (reader.Read() && reader.Depth >= entDepth)
            {
                //处理开始节点
                while (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "match")
                    {
                        ConditionalElement myWhen = new ConditionalElement();
                        myWhen.ReadXml(reader);
                        Match = myWhen;
                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "match")
                            reader.ReadEndElement();
                    }

                    if (reader.Name == "invoke")
                    {
                        reader.Read();
                        while (reader.NodeType == XmlNodeType.Element)
                        {
                            _innerSteps.ReadXmlEx<ModuleBuildStepElement>(reader);
                        }
                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "invoke")
                            reader.ReadEndElement();
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
            if (!string.IsNullOrEmpty(ModuleId))
                writer.WriteAttributeString("moduleId", ModuleId);

            writer.WriteAttributeString("type", Type);
            writer.WriteAttributeString("enumerator", ScopeIEnumerableInstance);

            if (BuildInstance)
                writer.WriteAttributeString("isInstance", BuildInstance.ToString());
            writer.WriteAttributeString("target", Target.ToString());

            writer.WriteStartElement("match");
            Match.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("invoke");
            writer.WriteAttributeString("type", typeof(ModuleBuildStepElement).GetNoVersionTypeName());
            foreach (ModuleBuildStepElement step in Steps)
            {
                step.ObjectWriteXml(writer);
            }
            writer.WriteEndElement();

        }

    }
}
