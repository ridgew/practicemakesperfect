using System;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XmlClrLan
{
    [Serializable]
    [XmlRoot(ElementName = "param")]
    [System.Diagnostics.DebuggerDisplay("Type:{Type}, Value:{Value}")]
    public class TypeValueElement : IXmlSerializable
    {
        /// <summary>
        /// 数值类型
        /// </summary>
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// 原始设置值
        /// </summary>
        [XmlAttribute(AttributeName = "value")]
        public object Value { get; set; }

        object RuntimeValue = null; //执行对象

        /// <summary>
        /// 是否是静态字段
        /// </summary>
        [XmlAttribute(AttributeName = "staticField")]
        public bool StaticField { get; set; }

        public Type GetClrType()
        {
            return TypeCache.GetRuntimeType(Type);
        }

        /// <summary>
        /// 获取运行域内的值
        /// </summary>
        /// <returns></returns>
        public virtual object GetObjectValue(ModuleRunScope scope = null)
        {
            if (Value == null)
                return null;

            Type targetType = TypeCache.GetRuntimeType(Type);

            #region 如果是静态委托设置
            if (targetType.IsSubclassOf(typeof(Delegate)))
            {
                string delegateStr = Value.ToString();
                int idx = delegateStr.LastIndexOf('.');
                int idxSep = delegateStr.IndexOf(',');

                if (idxSep != -1 && idx > idxSep) //程序集有字段分隔符号.
                {
                    idx = delegateStr.Substring(0, idxSep).LastIndexOf('.');
                }

                string setTypeStr = delegateStr.Substring(0, idx);
                string methodName = delegateStr.Substring(idx + 1);

                if (idxSep != -1)
                {
                    setTypeStr += delegateStr.Substring(idxSep);
                    methodName = delegateStr.Substring(idx + 1, idxSep - idx - 1).Trim();
                }

                Type setType = TypeCache.GetRuntimeType(setTypeStr);
                if (StaticField)
                {
                    FieldInfo mInfo = setType.GetField(methodName, BindingFlags.Public | BindingFlags.Static);
                    if (mInfo == null)
                    {
                        throw new System.Configuration.ConfigurationErrorsException("字段[" + methodName + "]在类型(" + setType.FullName + ")中未找到，请确保系统配置正确！");
                    }
                    else
                    {
                        return mInfo.GetValue(null);
                    }
                }
                else
                {
                    MethodInfo mInfo = setType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
                    if (mInfo == null)
                    {
                        throw new System.Configuration.ConfigurationErrorsException("函数[" + methodName + "]在类型(" + setType.FullName + ")中未找到，请确保委托配置正确！");
                    }
                    else
                    {
                        return Delegate.CreateDelegate(targetType, mInfo, true);
                    }
                }
            }
            #endregion

            if (TypeCache.IsBasicType(targetType))
            {
                RuntimeValue = Value;

             FoundValueInScope:
                if (targetType == typeof(Type))
                    return TypeCache.GetRuntimeType(Value.ToString());

                if (RuntimeValue != null && RuntimeValue.GetType() == typeof(string))
                {
                    string valDef = RuntimeValue.ToString();
                    if (valDef.IndexOf('$') != -1 && scope != null)
                    {
                        if (valDef == "$StepSwap")
                            RuntimeValue = scope.StepSwap;

                        goto FoundValueInScope;
                    }
                    else
                    {
                        if (targetType == typeof(string))
                            return valDef;
                    }
                }
                return Convert.ChangeType(RuntimeValue, targetType);
            }

            return null;
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
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 <see cref="T:System.Xml.XmlWriter" /> 流。</param>
        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("type", Type);
            writer.WriteAttributeString("value", Value.ToString());
            if (StaticField)
                writer.WriteAttributeString("staticField", StaticField.ToString());
        }
    }
}
