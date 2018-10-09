using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace XmlClrLan
{
    public static class ReflectUtil
    {
        public static bool MethodHasAttribute<T1, T2>(Expression<Func<T1, object>> expression)
        {
            var type = typeof(T1);
            string name = GetMethodName(expression.Body);
            Type[] args = GetArgTypes(expression.Body).ToArray();
            var methodInfo = type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, args, null);
            var attributes = methodInfo.GetCustomAttributes(typeof(T2), true);
            return attributes.Any();
        }

        public static string GetMethodName(Expression expression)
        {
            var callExpression = expression as MethodCallExpression;
            if (callExpression != null)
            {
                var methodCallExpression = callExpression;
                return methodCallExpression.Method.Name;
            }
            throw new Exception("Could not determine member from " + expression);
        }

        public static IEnumerable<Type> GetArgTypes(Expression expression)
        {
            var callExpression = expression as MethodCallExpression;
            if (callExpression != null)
            {
                var methodCallExpression = callExpression;
                ReadOnlyCollection<Expression> args = methodCallExpression.Arguments;
                return args.Select(x => x.Type);
            }

            throw new Exception("Could not determine member from " + expression);
        }

        /// <summary>
        /// 转换为绑定标识
        /// </summary>
        public static BindingFlags AsBindingFlags(this string enumStr)
        {
            if (Regex.IsMatch(enumStr, "^\\d+$", RegexOptions.IgnoreCase))
            {
                BindingFlags flag = (BindingFlags)(int.Parse(enumStr));
                return flag;
            }
            else
            {
                BindingFlags myFlags = BindingFlags.Default;
                string[] flagSetting = enumStr.Split('|', ','); //多个字段系统使用,代码运算是|
                foreach (string fItem in flagSetting)
                {
                    myFlags = myFlags | (BindingFlags)Enum.Parse(typeof(BindingFlags), fItem.Trim(), true);
                }
                return myFlags;
            }
        }

        public static T GetProperty<T>(object obj, string name)
        {
            return (T)obj.GetType().GetProperties().First(x => x.Name == name).GetValue(obj, null);
        }

        public static bool HasProperty(object obj, string name)
        {
            return obj.GetType().GetProperties().Any(x => x.Name == name);
        }

        public static bool IsPublicNonDefaultCtor(this Type type)
        {
            var ctors = type.GetConstructors().Where(x => x.GetParameters().Any());
            return ctors.Any();
        }

        public static bool IsDefaultCtor(this Type type)
        {
            var ctors = type.GetConstructors();
            return ctors.Any(x => !x.GetParameters().Any());
        }

        public static bool HasInterface<T>(this Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(T));
        }

        public static string GetFriendlyName(this Type type)
        {
            if (type.IsGenericType)
            {
                var sb = new StringBuilder();
                sb.Append(type.FullName.Remove(type.FullName.IndexOf('`')));
                sb.Append("<");
                Type[] arguments = type.GetGenericArguments();
                for (int i = 0; i < arguments.Length; i++)
                {
                    sb.Append(arguments[i].GetFriendlyName());
                    if (i + 1 < arguments.Length)
                    {
                        sb.Append(", ");
                    }
                }
                sb.Append(">");
                return sb.ToString();
            }
            return type.FullName;
        }

        /// <summary>
        /// 从配置静态方法创建符合特定委托类型的委托
        /// </summary>
        /// <typeparam name="TDelegate">委托类型</typeparam>
        /// <param name="staticCfgMethod">特定类型的静态方法描述，形如ClrServiceHost.Management.Communication::GetApplicationList, ClrServiceHost。</param>
        /// <returns></returns>
        public static TDelegate CreateFromConfig<TDelegate>(this string staticCfgMethod)
            where TDelegate : class
        {
            string pattern = "::([^,]+)";
            Match m = Regex.Match(staticCfgMethod, pattern, RegexOptions.IgnoreCase);
            if (!m.Success)
            {
                throw new System.Configuration.ConfigurationErrorsException("服务端获取通信配置的委托方法(" + staticCfgMethod + ")配置错误！");
            }
            else
            {
                Type methodType = Type.GetType(staticCfgMethod.Replace(m.Value, string.Empty));
                MethodInfo method = methodType.GetMethod(m.Groups[1].Value, BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                return Delegate.CreateDelegate(typeof(TDelegate), method, true) as TDelegate;
            }
        }

        /// <summary>
        /// 转换为委托字符串形式
        /// </summary>
        public static string ToDelegateString(this MemberInfo targetMethod)
        {
            Type targetType = targetMethod.ReflectedType;
            return string.Format("{0}::{1}, {2}", targetType, targetMethod.Name,
                Path.GetFileNameWithoutExtension(targetType.Assembly.Location) //targetType.Assembly.FullName.TrimAfter(",")
                );
        }

        /// <summary>
        /// 获取不包含版本的类型全称，形如：BizService.Interface.Services.LogService, BizService.Interface。
        /// </summary>
        /// <param name="instanceType">对象类型</param>
        /// <returns></returns>
        public static string GetNoVersionTypeName(this Type instanceType)
        {
            if (!instanceType.IsGenericType)
            {
                return string.Format("{0}, {1}",
                instanceType.FullName,
                Path.GetFileNameWithoutExtension(instanceType.Assembly.Location));
            }
            else
            {
                string rawFullName = instanceType.FullName;
                string baseTypeName = rawFullName.Substring(0, rawFullName.IndexOf('`'));
                return string.Format("{0}<{1}>, {2}",
                    baseTypeName,
                    string.Join(",", Array.ConvertAll<Type, string>(instanceType.GetGenericArguments(),
                    t => t.ToSimpleType())),
                Path.GetFileNameWithoutExtension(instanceType.Assembly.Location));
            }

        }

        /// <summary>
        /// 转换到特定数值类型
        /// </summary>
        public static TData To<TData>(this string rawString)
        {
            Converter<string, TData> gConvert = new Converter<string, TData>(s =>
            {
                if (rawString == null)
                {
                    return default(TData);
                }
                else
                {
                    return (TData)Convert.ChangeType(s, typeof(TData));
                }
            });
            return gConvert(rawString);
        }

        /// <summary>
        /// 无异常转换到特定数值类型
        /// </summary>
        public static TData As<TData>(this string rawString)
        {
            TData result = default(TData);
            try
            {
                result = To<TData>(rawString);
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 二进制序列的16进制视图形式（16字节换行）
        /// </summary>
        public static string ByteArrayToHexString(this byte[] tBinBytes)
        {
            string draftStr = System.Text.RegularExpressions.Regex.Replace(BitConverter.ToString(tBinBytes),
            "([A-z0-9]{2}\\-){16}",
            m =>
            {
                return m.Value.Replace("-", " ") + Environment.NewLine;
            });
            return draftStr.Replace("-", " ");
        }

        /// <summary>
        /// 从原始16进制字符还原到字节序列
        /// </summary>
        public static byte[] HexPatternStringToByteArray(this string hexrawStr)
        {
            if (string.IsNullOrEmpty(hexrawStr))
            {
                return new byte[0];
            }

            string trueRaw = hexrawStr.Replace(" ", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("-", "")
                .Replace("\t", "").Trim();

            int totalLen = trueRaw.Length;
            if (totalLen % 2 != 0)
            {
                throw new InvalidCastException("hex string size invalid.");
            }
            else
            {
                byte[] rawBin = new byte[totalLen / 2];
                for (int i = 0; i < totalLen; i = i + 2)
                {
                    rawBin[i / 2] = Convert.ToByte(int.Parse(trueRaw.Substring(i, 2),
                        System.Globalization.NumberStyles.AllowHexSpecifier));
                }
                return rawBin;
            }
        }


        /// <summary>
        /// 获取对象序列化的XmlDocument版本
        /// </summary>
        /// <param name="pObj">对象实体</param>
        /// <param name="noNamespaceAttr">属性是否添加默认命名空间</param>
        /// <returns>如果对象实体为Null，则返回结果为Null。</returns>
        public static XmlDocument GetXmlDoc(this object pObj, bool noNamespaceAttr)
        {
            XmlDocument xml = new XmlDocument();
            string xmlContent = GetXmlDocString(pObj, noNamespaceAttr);
            xmlContent = xmlContent.XmlUtf8BOMClear();
            xml.LoadXml(xmlContent);
            return xml;
        }

        /// <summary>
        /// 移除XML文档前缀的UTF-8字节
        /// </summary>
        public static string XmlUtf8BOMClear(this string xmlStr)
        {
            #region UTF8 序列化编码字符
            if (xmlStr[0] != '<')
            {
                //EF BB BF EF BB BF 
                xmlStr = xmlStr.Substring(6);
                if (xmlStr[0] != '<')
                    xmlStr = "<?xml " + xmlStr;
            }
            #endregion
            return xmlStr;
        }

        public static string GetXmlDocString(this object pObj, bool noNamespaceAttr)
        {
            if (pObj == null) { return null; }
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xs = new XmlSerializer(pObj.GetType(), string.Empty);
                XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.UTF8);
                if (noNamespaceAttr)
                {
                    XmlSerializerNamespaces xn = new XmlSerializerNamespaces();
                    xn.Add("", "");
                    xs.Serialize(xtw, pObj, xn);
                }
                else
                {
                    xs.Serialize(xtw, pObj);
                }

                string xmlContent = Encoding.UTF8.GetString(ms.ToArray()).Trim();
                return xmlContent.XmlUtf8BOMClear();
            }
        }

        /// <summary>
        /// 获取对象序列化的XmlDocument版本
        /// </summary>
        /// <param name="pObj">对象实体</param>
        /// <returns>如果对象实体为Null，则返回结果为Null。</returns>
        public static XmlDocument GetXmlDoc(this object pObj)
        {
            return GetXmlDoc(pObj, false);
        }

        /// <summary>
        /// 从已序列化数据(XmlDocument)中获取对象实体
        /// </summary>
        /// <typeparam name="T">要返回的数据类型</typeparam>
        /// <param name="xmlDoc">已序列化的文档对象</param>
        /// <returns>对象实体</returns>
        public static T GetObject<T>(this XmlDocument xmlDoc)
        {
            if (xmlDoc == null) { return default(T); }
            XmlNodeReader xmlReader = new XmlNodeReader(xmlDoc.DocumentElement);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(xmlReader);
        }

        /// <summary>
        /// 输出带缩进格式的XML文档
        /// </summary>
        /// <param name="xDoc">XML文档对象</param>
        /// <param name="writer">文本输出器</param>
        public static void WriteIndentedContent(this XmlDocument xDoc, TextWriter writer)
        {
            XmlTextWriter xWriter = new XmlTextWriter(writer);
            xWriter.Formatting = Formatting.Indented;
            xDoc.WriteContentTo(xWriter);
        }

        /// <summary>
        /// 直接调用内部对象的方法/函数(支持重载调用)
        /// </summary>
        /// <param name="refType">目标数据类型</param>
        /// <param name="funName">函数名称，区分大小写。</param>
        /// <param name="funParams">函数参数信息</param>
        public static object InvokeFunction(Type refType, string funName, params object[] funParams)
        {
            return InvokeMethodOrGetProperty(refType, funName, null, funParams);
        }

        /// <summary>
        /// 直接调用内部对象的方法/函数或获取属性(支持重载调用)
        /// </summary>
        /// <param name="refType">目标数据类型</param>
        /// <param name="funName">函数名称，区分大小写。</param>
        /// <param name="objInitial">如果调用属性，则为相关对象的初始化数据，否则为Null。</param>
        /// <param name="funParams">函数参数信息</param>
        /// <returns>运行结果</returns>
        public static object InvokeMethodOrGetProperty(Type refType, string funName, object[] objInitial, params object[] funParams)
        {
            MemberInfo[] mis = refType.GetMember(funName);
            if (mis.Length < 1)
            {
                throw new InvalidProgramException(string.Concat("函数/方法 [", funName, "] 在指定类型(", refType.ToString(), ")中不存在！"));
            }
            else
            {
                MethodInfo targetMethod = null;
                StringBuilder pb = new StringBuilder();
                foreach (MemberInfo mi in mis)
                {
                    if (mi.MemberType != MemberTypes.Method)
                    {
                        if (mi.MemberType == MemberTypes.Property)
                        {
                            #region 调用属性方法Get
                            PropertyInfo pi = (PropertyInfo)mi;
                            targetMethod = pi.GetGetMethod();
                            break;
                            #endregion
                        }
                        else
                        {
                            throw new InvalidProgramException(string.Concat("[", funName, "] 不是有效的函数/属性方法！"));
                        }
                    }
                    else
                    {
                        #region 检查函数参数和数据类型 绑定正确的函数到目标调用
                        bool validParamsLen = false, validParamsType = false;

                        MethodInfo curMethod = (MethodInfo)mi;
                        ParameterInfo[] pis = curMethod.GetParameters();
                        if (pis.Length == funParams.Length)
                        {
                            validParamsLen = true;

                            pb = new StringBuilder();
                            bool paramFlag = true;
                            int paramIdx = 0;

                            #region 检查数据类型 设置validParamsType是否有效
                            foreach (ParameterInfo pi in pis)
                            {
                                pb.AppendFormat("Parameter {0}: Type={1}, Name={2}\n", paramIdx, pi.ParameterType, pi.Name);

                                //不对Null和接受Object类型的参数检查
                                if (funParams[paramIdx] != null && pi.ParameterType != typeof(object) &&
                                     (pi.ParameterType != funParams[paramIdx].GetType()))
                                {
                                    #region 检查类型是否兼容
                                    try
                                    {
                                        funParams[paramIdx] = Convert.ChangeType(funParams[paramIdx], pi.ParameterType);
                                    }
                                    catch (Exception)
                                    {
                                        paramFlag = false;
                                    }
                                    #endregion
                                    //break;
                                }
                                ++paramIdx;
                            }
                            #endregion

                            if (paramFlag == true)
                            {
                                validParamsType = true;
                            }
                            else
                            {
                                continue;
                            }

                            if (validParamsLen && validParamsType)
                            {
                                targetMethod = curMethod;
                                break;
                            }
                        }
                        #endregion
                    }
                }

                if (targetMethod != null)
                {
                    object objReturn = null;
                    #region 兼顾效率和兼容重载函数调用
                    try
                    {
                        object objInstance = System.Activator.CreateInstance(refType, objInitial);
                        objReturn = targetMethod.Invoke(objInstance, BindingFlags.InvokeMethod, Type.DefaultBinder, funParams,
                            System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        objReturn = refType.InvokeMember(funName, BindingFlags.InvokeMethod, Type.DefaultBinder, null, funParams);
                    }
                    #endregion
                    return objReturn;
                }
                else
                {
                    pb.AppendLine("---------------------------------------------");
                    pb.AppendLine("传递参数信息：");
                    foreach (object fp in funParams)
                    {
                        pb.AppendFormat("Type={0}, value={1}\n", fp.GetType(), fp);
                    }

                    throw new InvalidProgramException(string.Concat("函数/方法 [", refType.ToString(), ".", funName,
                        "(args ...) ] 参数长度和数据类型不正确！\n 引用参数信息参考：\n",
                        pb.ToString()));
                }
            }

        }

        public static string ObjectFormat(this string nvFormat, string pattern, int group, Func<string, string, string> fieldValConverter)
        {
            MatchEvaluator evaluator = null;
            if (fieldValConverter != null)
            {
                string str = @"\{([a-zA-z_$][a-zA-z_0-9$]*)\}";
                if (!string.IsNullOrEmpty(pattern))
                {
                    str = pattern;
                }
                else
                {
                    group = 1;
                }
                if (!Regex.IsMatch(nvFormat, str, RegexOptions.IgnoreCase))
                {
                    return nvFormat;
                }
                if (evaluator == null)
                {
                    evaluator = m => fieldValConverter(m.Groups[group].Value, m.Value);
                }
                nvFormat = Regex.Replace(nvFormat, str, evaluator);
            }
            return nvFormat;
        }

        public static string NameValueFormat(this string nvFormat, string pattern, int group, NameValueCollection nv)
        {
            if (nv == null)
            {
                return nvFormat;
            }
            return nvFormat.ObjectFormat(pattern, group, (K, M) => (nv[K] ?? M));
        }

        public static string PropertyFormat(this string propertyFormat, string pattern, int group, object instance)
        {
            MatchEvaluator evaluator = null;
            Type instanceType;
            if (instance != null)
            {
                string str = @"\{([a-zA-z_$][a-zA-z_0-9$]*)\}";
                if (!string.IsNullOrEmpty(pattern))
                {
                    str = pattern;
                }
                else
                {
                    group = 1;
                }
                instanceType = instance.GetType();
                if (!Regex.IsMatch(propertyFormat, str, RegexOptions.IgnoreCase))
                {
                    return propertyFormat;
                }
                if (evaluator == null)
                {
                    evaluator = delegate (Match m)
                    {
                        PropertyInfo property = instanceType.GetProperty(m.Groups[group].Value);
                        if (property == null)
                        {
                            return m.Value;
                        }
                        object obj2 = property.GetValue(instance, null);
                        if (obj2 != null)
                        {
                            return obj2.ToString();
                        }
                        return "";
                    };
                }
                propertyFormat = Regex.Replace(propertyFormat, str, evaluator);
            }
            return propertyFormat;
        }

    }
}
