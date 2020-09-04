using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SMOMDeploy
{
    public class SimpleExpression
    {
        public SimpleExpression(string expressionDefine)
        {
            myExpression = expressionDefine;
        }

        string myExpression;

        /// <summary>
        /// 包含[]或()等需要子解析的表达式
        /// </summary>
        /// <param name="expression">当前字段或表达式</param>
        /// <param name="firstChar">首个需要解析的字符</param>
        /// <returns>检索到的位置</returns>
        int ContainSubParseIndex(string expression, out char firstChar)
        {
            firstChar = '\0';
            int idx = -1;
            char[] totalChars = expression.ToCharArray();
            for (int i = 0; i < totalChars.Length; i++)
            {
                char myChar = totalChars[i];
                if (myChar == '(' || myChar == '[')
                {
                    idx = i;
                    firstChar = myChar;
                    break;
                }
            }
            return idx;
        }

        internal static int PairCharEnd(string source, int startIndex, char theChar)
        {
            if (source[startIndex] != theChar)
                throw new ArgumentException($"定位{startIndex}的字符不是{theChar}!");

            char[] sChars = new char[] { '"', '{', '\'', '(' };
            char[] eChars = new char[] { '"', '}', '\'', ')' };

            int sIndx = Array.IndexOf(sChars, theChar);
            char endChar = eChars[sIndx];
            int sourceIndex = startIndex;

        fetchNext:
            int endIndex = source.IndexOf(endChar, startIndex + 1);
            if (endIndex != -1 && source[endIndex - 1] == '\\')
            {
                startIndex = endIndex;
                goto fetchNext;
            }


            int midIndex = sourceIndex + 1;
            int mEndIndex = endIndex;

        FetchMiddle:
            string rangStr = source.Substring(midIndex, mEndIndex - midIndex);
            int mIdx = rangStr.IndexOf(theChar);
            if (mIdx != -1)
            {
                if (rangStr[mIdx - 1] != '\\')
                {
                    mEndIndex = source.IndexOf(endChar, mEndIndex + 1);
                    if (mEndIndex == -1)
                        throw new ArgumentException($"数据格式错误，从{mEndIndex}开始，没能找到{endChar}！");
                }
                midIndex += mIdx + 1;
                goto FetchMiddle;
            }

            endIndex = mEndIndex;
            return endIndex;
        }

        /// <summary>
        /// 获取绑定实例的属性或字段值
        /// </summary>
        /// <param name="instance">属性实例</param>
        /// <param name="expression">字段名称或简单表达式</param>
        /// <returns></returns>
        object ParseInstanceExpression(object instance, string expression)
        {
            Type instanceType = instance.GetType();
            object objVal = null;
            string[] totalFields = expression.Split('.');

            for (int i = 0, j = totalFields.Length; i < j; i++)
            {
                if (i > 0)
                {
                    instance = objVal;
                    if (objVal != null) instanceType = objVal.GetType();
                }

                string subFieldName = totalFields[i];

                #region 多级属性读取
                Char myChar;
                int cIndex = ContainSubParseIndex(subFieldName, out myChar);
                if (cIndex == -1)
                {
                    #region 无须子解析的属性或字段
                    //属性
                    PropertyInfo pi = instanceType.GetProperty(subFieldName);
                    if (pi != null)
                    {
                        objVal = pi.GetValue(instance, null);
                        continue;
                    }

                    //字段
                    FieldInfo fi = instanceType.GetField(subFieldName);
                    if (fi != null)
                    {
                        objVal = fi.GetValue(instance);
                        continue;
                    }

                    //索引属性
                    PropertyInfo itemProp = instanceType.GetProperty("Item");
                    if (itemProp != null)
                    {
                        objVal = itemProp.GetValue(instance, new object[] { subFieldName });
                    }
                    #endregion
                }
                else
                {
                    int parsedLength = 0;
                    int totalLen = subFieldName.Length;
                    string rawExpression = subFieldName;
                    do
                    {
                        #region 循环解析字段
                        if (parsedLength > 0)
                        {
                            instance = objVal;
                            if (objVal != null) instanceType = objVal.GetType();
                            subFieldName = rawExpression.Substring(parsedLength);
                            myChar = subFieldName[0];
                        }

                        if (myChar == '(')
                        {
                            #region 函数
                            string funName = subFieldName.Substring(0, cIndex);
                            Type[] argumentTypes = Type.EmptyTypes;
                            int fundEndIndex = PairCharEnd(subFieldName, cIndex, '(');
                            string argumentStr = subFieldName.Substring(cIndex + 1, fundEndIndex - cIndex - 1);
                            object[] argumentsObj = null;
                            if (!string.IsNullOrEmpty(argumentStr))
                            {
                                #region 组建参数和参数类型
                                string[] arguments = argumentStr.Split(',');
                                List<object> args = new List<object>();
                                List<Type> argTypes = new List<Type>();
                                foreach (string arg in arguments)
                                {
                                    if (arg.StartsWith("'") && arg.EndsWith("'"))
                                    {
                                        args.Add(arg.Trim('\'')[0]);
                                        argTypes.Add(typeof(char));
                                    }
                                    else if (arg.StartsWith("[") && arg.EndsWith("]"))
                                    {
                                        #region 字符或字符串数组
                                        string newArg = arg.Substring(1, arg.Length - 2);
                                        if (arg.Substring(1, 1)[0] == '\'')
                                        {
                                            var arrObj = Array.CreateInstance(typeof(char), 1);
                                            arrObj.SetValue(newArg.Trim('\'')[0], 0);
                                            argTypes.Add(typeof(char[]));
                                            args.Add(arrObj);
                                        }

                                        if (arg.Substring(1, 1)[0] == '"')
                                        {
                                            var arrObj = Array.CreateInstance(typeof(string), 1);
                                            arrObj.SetValue(newArg.Trim('"')[0], 0);
                                            argTypes.Add(typeof(string[]));
                                            args.Add(arrObj);
                                        }
                                        #endregion
                                    }
                                    else if (Regex.IsMatch(arg, "^\\d+$"))
                                    {
                                        args.Add(arg);
                                        argTypes.Add(typeof(int));
                                    }
                                    else
                                    {
                                        args.Add(arg);
                                        argTypes.Add(typeof(string));
                                    }
                                }
                                argumentTypes = argTypes.ToArray();
                                argumentsObj = args.ToArray();
                                #endregion
                            }

                            MethodInfo method = instanceType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).FirstOrDefault(m =>
                            {
                                if (m.Name == funName)
                                {
                                    ParameterInfo[] parameters = m.GetParameters();
                                    if (argumentTypes.Length == parameters.Length)
                                    {
                                        if (parameters.Length > 0)
                                        {
                                            for (int p = 0, q = parameters.Length; p < q; p++)
                                            {
                                                ParameterInfo parameter = parameters[p];
                                                if (!parameter.ParameterType.Equals(argumentTypes[p]))
                                                    return false;
                                            }
                                        }
                                        return true;
                                    }
                                }
                                return false;
                            });

                            if (method == null)
                                throw new ArgumentNullException($"{instanceType.FullName}没找到满足当前参数列表的函数定义{funName}!");

                            objVal = method.Invoke(instance, argumentsObj);
                            parsedLength += funName.Length + argumentStr.Length + "()".Length;
                            #endregion
                        }
                        else if (myChar == '[')
                        {
                            int pIdx = subFieldName.IndexOf('[');
                            if (pIdx > 0)
                            {
                                objVal = ParseInstanceExpression(instance, subFieldName.Substring(0, cIndex));
                                if (objVal == null)
                                    throw new ArgumentNullException($"找不到表达式{subFieldName.Substring(0, cIndex)}所表示的实例！");
                                instance = objVal;
                                if (objVal != null) instanceType = objVal.GetType();
                            }

                            #region 数组或成员
                            int pIdxEnd = subFieldName.IndexOf(']', pIdx + 1);
                            if (pIdxEnd > pIdx)
                            {
                                string idxOrName = subFieldName.Substring(pIdx + 1, pIdxEnd - pIdx - 1);
                                if (idxOrName.StartsWith("'") && idxOrName.EndsWith("'")) idxOrName = idxOrName.Trim('\'');
                                if (idxOrName.StartsWith("\"") && idxOrName.EndsWith("\"")) idxOrName = idxOrName.Trim('"');
                                objVal = GetSubIndexOrProperty(idxOrName, instance);
                                parsedLength += pIdxEnd + 1;
                            }
                            #endregion
                        }
                        #endregion
                    }
                    while (parsedLength < totalLen);

                }
                #endregion
            }

            return objVal;
        }

        /// <summary>
        /// 获取绑定实例的索引命名属性
        /// </summary>
        /// <param name="idxName">索引名称或序号</param>
        /// <param name="instance">绑定实例</param>
        /// <returns></returns>
        object GetSubIndexOrProperty(string idxName, object instance)
        {
            if (instance == null)
                throw new ArgumentNullException($"获取实例的{idxName}时发现实例为NULL!");

            Type instanceType = instance.GetType();
            object objVal = null;
            bool isNum = Regex.IsMatch(idxName, "^\\d+$");
            MethodInfo mInfo = instanceType.GetMethod("Get", new Type[] { isNum ? typeof(int) : typeof(string) });
            if (mInfo != null)
            {
                if (isNum)
                {
                    objVal = mInfo.Invoke(instance, new object[] { Convert.ToInt32(idxName) });
                }
                else
                {
                    objVal = mInfo.Invoke(instance, new object[] { idxName });
                }
            }
            else
            {
                if (!(instance is Array))
                {
                    objVal = ParseInstanceExpression(instance, idxName);
                }
                else
                {
                    var arrInstance = instance as Array;
                    #region 数组或子属性
                    if (isNum)
                    {
                        int itemIdx = Convert.ToInt32(idxName);
                        if (itemIdx < arrInstance.Length)
                            objVal = arrInstance.GetValue(itemIdx);
                    }
                    else
                    {
                        Type subArrType = instanceType.GetElementType();
                        //@ServiceName='ftp'
                        Match m = Regex.Match(idxName, "@(?<prop>[\\w]+)='(?<val>[\\w]+)'");
                        if (m.Success)
                        {
                            bool valueGot = false;
                            #region 属性查询
                            PropertyInfo prop = subArrType.GetProperty(m.Groups["prop"].Value);
                            if (prop != null)
                            {
                                for (int i = 0; i < arrInstance.Length; i++)
                                {
                                    var item = arrInstance.GetValue(i);
                                    if (prop.GetValue(item, null).ToString() == m.Groups["val"].Value)
                                    {
                                        objVal = item;
                                        valueGot = true;
                                        break;
                                    }
                                }
                            }
                            #endregion

                            #region 字段查询
                            if (!valueGot)
                            {
                                FieldInfo field = subArrType.GetField(m.Groups["prop"].Value);
                                if (field != null)
                                {
                                    for (int i = 0; i < arrInstance.Length; i++)
                                    {
                                        var item = arrInstance.GetValue(i);
                                        if (field.GetValue(item).ToString() == m.Groups["val"].Value)
                                        {
                                            objVal = item;
                                            break;
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
            }
            return objVal;
        }

        /// <summary>
        /// 获取表达式运算结果
        /// </summary>
        /// <returns>System.Object.</returns>
        public object EvalResult(object topInstance)
        {
            if (myExpression.StartsWith("this."))
            {
                return ParseInstanceExpression(topInstance, myExpression.Substring(5));
            }
            else
            {
                return ParseInstanceExpression(topInstance, myExpression);
            }
        }
    }
}
