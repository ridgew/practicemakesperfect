using System;
using System.Collections.Generic;

namespace XmlClrLan
{
    public class ModuleRunScope : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleRunScope"/> class.
        /// </summary>
        public ModuleRunScope()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleRunScope"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public ModuleRunScope(ModuleRunScope parent)
        {
            Parent = parent;
        }

        [NonSerialized]
        public Exception LastError;

        /// <summary>
        /// 父级作用域
        /// </summary>
        public ModuleRunScope Parent;

        List<string> stackList = new List<string>();

        /// <summary>
        /// 作用域执行标记
        /// </summary>
        public void AddStackFrame(string frameId)
        {
            stackList.Add(frameId);
        }

        /// <summary>
        /// 分步数据存储
        /// </summary>
        [NonSerialized]
        public object StepSwap;

        public void Dispose()
        {
            stackList.Clear();
            VarDict.Clear();
        }

        Dictionary<string, object> VarDict = new Dictionary<string, object>();

        public object GetVaraible(string varName)
        {
            //键值特殊处理$ TODO
            if (varName == "$StepSwap")
                return StepSwap;

            if (VarDict.ContainsKey(varName))
            {
                return VarDict[varName];
            }
            else
            {
                if (Parent == null)
                    return null;

                return Parent.GetVaraible(varName);
            }

        }

        /// <summary>
        /// 设置运行区间变量
        /// </summary>
        public ModuleRunScope SetVariable(string varName, object val)
        {
            if (VarDict.ContainsKey(varName))
                VarDict[varName] = val;
            else
                VarDict.Add(varName, val);

            return this;
        }

    }
}
