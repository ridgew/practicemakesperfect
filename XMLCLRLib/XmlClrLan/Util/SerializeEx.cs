using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace XmlClrLan
{
    /// <summary>
    /// 序列化扩展
    /// </summary>
    public static class SerializeEx
    {
        /// <summary>
        /// 基于接口/基类实现的子类型写入
        /// </summary>
        /// <typeparam name="TBase">接口/基类类型</typeparam>
        /// <param name="children">子类型实例列表</param>
        /// <param name="writer">序列化写入器</param>
        public static void WriteXmlEx<TBase>(this List<TBase> children, XmlWriter writer)
        {
            XmlSerializerNamespaces xn = new XmlSerializerNamespaces();
            xn.Add("", "");
            string rawChildStr = null;

            foreach (TBase item in children)
            {
                Type t = item.GetType();
                StringBuilder cb = new StringBuilder();
                using (StringWriter sw = new System.IO.StringWriter(cb))
                {
                    XmlSerializer xs = new XmlSerializer(t);
                    xs.Serialize(sw, item, xn);
                    sw.Close();
                }
                rawChildStr = cb.ToString();

                XmlDocument rawDoc = new XmlDocument();
                rawDoc.LoadXml(rawChildStr);

                XmlAttribute attr = rawDoc.CreateAttribute("type");
                attr.Value = t.GetNoVersionTypeName();

                rawDoc.DocumentElement.Attributes.Append(attr);
                writer.WriteRaw(rawDoc.DocumentElement.OuterXml);
            }
        }

        /// <summary>
        /// 读到下一个指定类型节点
        /// </summary>
        /// <param name="reader">序列化读取器</param>
        /// <param name="nodeType">节点类型</param>
        public static void ReadToNodeType(this XmlReader reader, XmlNodeType nodeType)
        {
            int entDepth = reader.Depth;
            while (reader.Read()
                && reader.Depth >= entDepth)
            {
                if (reader.NodeType == nodeType)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 读取到特定节点名称(没有名称则为任一节点),忽略其他节点类型
        /// </summary>
        /// <param name="reader">序列化读取器</param>
        /// <param name="elementName">节点名称</param>
        public static void ReadToElement(this XmlReader reader, string elementName)
        {
            if (!string.IsNullOrEmpty(elementName) && reader.NodeType == XmlNodeType.Element && reader.Name == elementName)
                return;

            int entDepth = reader.Depth;
            while (reader.Read() && reader.Depth >= entDepth)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (string.IsNullOrEmpty(elementName))
                    {
                        break;
                    }
                    else
                    {
                        if (reader.Name.Equals(elementName))
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 基于接口/基类实现的子类型读取
        /// </summary>
        /// <typeparam name="TBase">接口/基类类型</typeparam>
        /// <param name="children">子类型实例列表</param>
        /// <param name="reader">序列化读取器</param>
        /// <returns></returns>
        public static void ReadXmlEx<TBase>(this List<TBase> children, XmlReader reader)
            where TBase : class
        {
            string cTypeVal = null;
            Type cType = null;
            TBase subChildInstance = default(TBase);
            string interfaceTypeString = typeof(TBase).FullName;

            #region 处理节点
            if (reader.NodeType == XmlNodeType.Element)
            {
                cTypeVal = reader.GetAttribute("type");
                if (string.IsNullOrEmpty(cTypeVal))
                {
                    if (!ContainerConfiguration.IsKnownTaskByElementName(reader.Name, ref cType))
                    {
                        cType = typeof(TBase);
                        if (cType.IsInterface || cType.IsAbstract)
                        {
                            throw new System.Configuration.ConfigurationErrorsException(string.Format("抽象类型或接口{1}不能序列化，节点名称{0}没有执行类型属性(type)!",
                                reader.Name, cType.FullName));
                        }
                    }
                }
                else
                {
                    cType = TypeCache.GetRuntimeType(cTypeVal);
                    if (typeof(TBase).IsInterface)
                    {
                        if (cType.GetInterface(interfaceTypeString, false) == null)
                        {
                            throw new System.Configuration.ConfigurationErrorsException(string.Format("类型{0}不是接口{1}的实现类!", cType.FullName, interfaceTypeString));
                        }
                    }
                    else
                    {
                        if (!cType.IsSubclassOf(typeof(TBase)))
                        {
                            throw new System.Configuration.ConfigurationErrorsException(string.Format("类型{0}不是{1}的继承类!", cType.FullName, interfaceTypeString));
                        }
                    }
                }

                XmlSerializer xChildTask = new XmlSerializer(cType, new XmlRootAttribute(reader.Name));
                subChildInstance = xChildTask.Deserialize(reader) as TBase;
                if (subChildInstance != null)
                    children.Add(subChildInstance);
            }
            #endregion

        }

        /// <summary>
        /// 写入实例的xml序列化
        /// </summary>
        /// <param name="instance">当前实例</param>
        /// <param name="writer">写入器</param>
        /// <param name="rootName">根节点名称</param>
        public static void ObjectWriteXml(this object instance, XmlWriter writer, string rootName)
        {
            XmlSerializerNamespaces xn = new XmlSerializerNamespaces();
            xn.Add("", "");
            XmlSerializer xsc = null;
            if (string.IsNullOrEmpty(rootName))
            {
                xsc = new XmlSerializer(instance.GetType());
            }
            else
            {
                xsc = new XmlSerializer(instance.GetType(), new XmlRootAttribute(rootName));
            }
            xsc.Serialize(writer, instance, xn);
        }

        /// <summary>
        /// 写入实例的xml序列化（含根节点)
        /// </summary>
        /// <param name="instance">当前实例</param>
        /// <param name="writer">写入器</param>
        public static void ObjectWriteXml(this object instance, XmlWriter writer)
        {
            ObjectWriteXml(instance, writer, null);
        }

        /// <summary>
        /// 读取为特定实例
        /// </summary>
        /// <typeparam name="T">要返回的数据类型</typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static T ObjectReadXml<T>(this XmlReader reader)
        {
            return (T)ObjectReadXmlType(reader, typeof(T));
        }

        public static object ObjectReadXmlType(this XmlReader reader, Type objType)
        {
            XmlSerializer serializer = new XmlSerializer(objType, new XmlRootAttribute(reader.Name));
            return serializer.Deserialize(reader);
        }

    }

}