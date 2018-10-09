using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Linq;
using System.Xml.Serialization;

namespace XmlClrLan
{
    /// <summary>
    /// 模块构建节点配置
    /// </summary>
    /// <seealso cref="System.Xml.Serialization.IXmlSerializable" />
    [Serializable]
    [XmlRoot(ElementName = "Builder")]
    public class SubModuleBuildElement : ModuleBuildStepElement, IExecutableInScope
    {
        bool _buildInstnce = true;

        /// <summary>
        /// 设置是否需要构建实例（默认为true）
        /// </summary>
        [XmlAttribute(AttributeName = "isInstance")]
        public bool BuildInstance
        {
            get { return _buildInstnce; }
            set { _buildInstnce = value; }
        }

        string moduleDef = null; //内部私有模块（默认继承父级）
        /// <summary>
        /// 模块命名标志(容器存在时共享）
        /// </summary>
        [XmlAttribute(AttributeName = "moduleId")]
        public string ModuleId
        {
            get
            {
                if (moduleDef == null && Module != null)
                    return Module.ModuleId;
                else
                    return moduleDef;
            }
            set
            {
                moduleDef = value;
            }
        }

        /// <summary>
        /// 模块类型
        /// </summary>
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        protected List<ModuleBuildStepElement> _innerSteps = new List<ModuleBuildStepElement>();

        /// <summary>
        /// 构建步骤
        /// </summary>
        [XmlIgnore]
        public ModuleBuildStepElement[] Steps
        {
            get { return _innerSteps.ToArray(); }
        }

        /// <summary>
        /// 构建目标类型
        /// </summary>
        [XmlAttribute(AttributeName = "target")]
        public BuildTarget Target { get; set; }

        /// <summary>
        /// 添加构建步骤
        /// </summary>
        /// <typeparam name="TStep">构建基础类型</typeparam>
        /// <param name="step">具体的实现</param>
        public void AddStep<TStep>(TStep step)
            where TStep : ModuleBuildStepElement
        {
            step.Module = this;
            _innerSteps.Add(step);
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
                        moduleDef = reader.Value;
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
                }
            }

            int entDepth = reader.Depth;
            while (reader.Read() && reader.Depth >= entDepth)
            {
                //处理开始节点
                while (reader.NodeType == XmlNodeType.Element)
                {
                    _innerSteps.ReadXmlEx<ModuleBuildStepElement>(reader);
                }
            }
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 <see cref="T:System.Xml.XmlWriter" /> 流。</param>
        public override void WriteXml(XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(moduleDef))
                writer.WriteAttributeString("moduleId", ModuleId);

            if (!string.IsNullOrEmpty(Type))
                writer.WriteAttributeString("type", Type);

            if (BuildInstance)
                writer.WriteAttributeString("isInstance", BuildInstance.ToString());

            writer.WriteAttributeString("target", Target.ToString());

            foreach (ModuleBuildStepElement step in _innerSteps)
            {
                step.ObjectWriteXml(writer);
            }
        }

        /// <summary>
        /// 构建实例对象
        /// </summary>
        protected object InvokeInScopeContructor(ModuleRunScope scope)
        {
            object instance = null;
            Type taregetType = TypeCache.GetRuntimeType(Type);
            ModuleConstructorElement constr = Steps.FirstOrDefault(s => s.GetType().Equals(typeof(ModuleConstructorElement))) as ModuleConstructorElement;

            #region 构建对象
            if (constr == null)
            {
                instance = Activator.CreateInstance(taregetType);
            }
            else
            {
                if (scope != null)
                    scope.AddStackFrame(constr.ToString());
                constr.InvokeInScope(taregetType, null, scope);
                instance = scope.StepSwap;
            }

            if (instance == null)
            {
                throw new System.Configuration.ConfigurationErrorsException("创建实例：(" + taregetType.FullName + ")失败，请确保系统配置正确！");
            }
            #endregion

            return instance;
        }

        /// <summary>
        /// 执行步骤（包含是否中途退出)
        /// </summary>
        /// <param name="taregetType">目标类型</param>
        /// <param name="instance">实例</param>
        /// <param name="scope">执行区间</param>
        /// <param name="mySteps">创建步骤</param>
        /// <returns></returns>
        protected virtual bool InvokeStepsInScope(Type taregetType, object instance, ModuleRunScope scope, params ModuleBuildStepElement[] mySteps)
        {
            bool returnResult = false;
            try
            {
                foreach (var step in mySteps)
                {
                    if (scope != null)
                        scope.AddStackFrame(step.ToString());

                    returnResult = step.InvokeInScope(taregetType, instance, scope);

                    if (returnResult)
                        break;
                }
            }
            catch (Exception ivkError)
            {
                scope.LastError = ivkError;
            }
            return returnResult;
        }

        /// <summary>
        /// 在作用域下执行
        /// </summary>
        /// <param name="instanceType">实例类型</param>
        /// <param name="instance">The instance.</param>
        /// <param name="scope">执行代码作用区间</param>
        public override bool InvokeInScope(Type instanceType, object instance, ModuleRunScope scope = null)
        {
            bool stepResult = InvokeStepsInScope(instanceType, instance, scope, Steps);
            return stepResult;
        }

        /// <summary>
        /// 在作用域下单独执行
        /// </summary>
        /// <param name="scope"></param>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">系统配置错误</exception>
        public virtual void InvokeInScope(ModuleRunScope scope)
        {
            if (BuildInstance)
            {
                Type taregetType = TypeCache.GetRuntimeType(Type);
                object instance = InvokeInScopeContructor(scope);
                InvokeStepsInScope(taregetType, instance, scope, Steps);
            }
            else
            {
                object instance = scope.StepSwap;
                Type taregetType = instance != null ? instance.GetType() : TypeCache.GetRuntimeType(Type);
                InvokeStepsInScope(taregetType, instance, scope, Steps);
            }
        }


        /// <summary>
        /// 获取作用域共享的子集模块构建，不再构建新实例
        /// </summary>
        /// <param name="type">子模块类型</param>
        /// <returns></returns>
        public static SubModuleBuildElement GetScopeSharedModule(string type)
        {
            SubModuleBuildElement module = new SubModuleBuildElement()
            {
                BuildInstance = false,
                Type = type,
                Target = BuildTarget.ScopeResult
            };
            return module;
        }

    }
}
