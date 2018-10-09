using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlClrLan;
using System.Diagnostics;
using DBHR.WinFormUnity;
using System.Reflection;
using DBHR.ClientModule;
using System.Windows.Forms;

namespace XMLCrlLanTest
{
    [TestClass]
    public class XmlClrLanDocTest
    {
        [TestMethod]
        public void ConfigTest()
        {
            ContainerConfiguration cfg = new ContainerConfiguration();
            List<ClientModuleElement> mList = new List<ClientModuleElement>();

            SubModuleBuildElement builder = new SubModuleBuildElement
            {
                Type = "DBHR.ClientModule.BaseEditForm, DBHR.ClientModule",
                ModuleId = "SP1010"
            };

            #region 方法调用
            builder.AddStep(new ModuleMethodCallElement
            {
                MethodName = "AddMainType",
                //When = new ConditionalElement {
                //    Executable = false
                //},
                Parameters = new ModuleConstructorElement
                {
                    Arguments = new TypeValueElement[]
                     {
                          new TypeValueElement {  Type="System.Type", Value = "DBHR.WinFormUnity.TestModule1, DBHR.WinFormUnity"}
                     }
                }
            });
            builder.AddStep(new ModuleMethodCallElement
            {
                MethodName = "AddMainType",
                Parameters = new ModuleConstructorElement
                {
                    Arguments = new TypeValueElement[]
                            {
                                new TypeValueElement {  Type="System.Type", Value = "DBHR.WinFormUnity.TestModule2, DBHR.WinFormUnity"}
                            }

                }
            });

            builder.AddStep(new ModuleMethodCallElement
            {
                MethodName = "AddDetailType",
                Parameters = new ModuleConstructorElement
                {
                    Arguments = new TypeValueElement[]
                            {
                                new TypeValueElement {  Type="System.Type", Value = "DBHR.WinFormUnity.TestModule1, DBHR.WinFormUnity"},
                                new TypeValueElement {  Type="System.Type", Value = "DBHR.WinFormUnity.TestModule2, DBHR.WinFormUnity"}
                            }

                }
            });
            builder.AddStep(new ModuleMethodCallElement { MethodName = "DoCreateFormControls" });
            #endregion

            #region 属性设置
            builder.AddStep(new ModulePropertyElement { Name = "MdiParent", Value = "$mainForm" });
            #endregion

            #region 方法调用
            builder.AddStep(new ModuleMethodCallElement { MethodName = "Show" });
            #endregion

            ClientModuleElement m = new ClientModuleElement()
            {
                Name = "SP",
                Type = "DBHR.ClientModule.StartUpModule, DBHR.ClientModule",
                MoubleBuild = new SubModuleBuildElement[] { builder }
            };

            mList.Add(m);

            cfg.Modules = mList.ToArray();
            XmlDocument xDoc = cfg.GetXmlDoc(true);
            var xmlstr = cfg.GetXmlDocString(true);
            //cfg.GetXmlDoc().WriteIndentedContent(Console.Out);

            var cfgObj = xDoc.GetObject<ContainerConfiguration>();
            string xmlStr2 = cfgObj.GetXmlDocString(true);
            Assert.AreEqual(xmlstr.XmlUtf8BOMClear(), xmlStr2.XmlUtf8BOMClear());

        }

        [TestMethod]
        public void ModuleMethodCallElementTest()
        {
            ModuleMethodCallElement ele = GetParamValueElement();
            XmlDocument xDoc = ele.GetXmlDoc(true);
            string xmlStr = xDoc.GetXmlDocString(true);
            Trace.Write(xmlStr);

            ModuleMethodCallElement restore = xDoc.GetObject<ModuleMethodCallElement>();
            string xmlStr2 = restore.GetXmlDocString(true);
            Trace.Write(xmlStr2);

            Assert.AreEqual(xmlStr.XmlUtf8BOMClear(), xmlStr2.XmlUtf8BOMClear());
        }

        [TestMethod]
        public void ConditionalTest()
        {
            ConditionalElement when = new ConditionalElement();
            when.AddItem(new StringConditionItem
            {
                ExpressionLeft = TempStrGetExpression(),
                ExpressionRight = (StringExpressItem)"1",
            });

            using (ModuleRunScope scope = new ModuleRunScope())
            {
                Assert.IsTrue(when.CanRunInScope(scope));
            }

            XmlDocument xDoc = when.GetXmlDoc(true);
            string xmlStr = xDoc.GetXmlDocString(true);
            Trace.Write(xmlStr);

            ConditionalElement restore = xDoc.GetObject<ConditionalElement>();
            string xmlStr2 = restore.GetXmlDocString(true);
            Trace.Write(xmlStr2);

            Assert.AreEqual(xmlStr.XmlUtf8BOMClear(), xmlStr2.XmlUtf8BOMClear());
        }

        ModuleMethodCallElement GetParamValueElement()
        {
            return new ModuleMethodCallElement
            {
                MethodName = "GetParamValue",
                Parameters = new ModuleConstructorElement
                {
                    Arguments = new TypeValueElement[] {
                        new TypeValueElement { Type ="string", Value = "KQ043001" },
                        new TypeValueElement { Type ="bool", Value = false }
                    }
                }
            };
        }

        StepBasedExpressionItem<string> TempStrGetExpression()
        {
            StepBasedExpressionItem<string> item = new StepBasedExpressionItem<string>();

            SubModuleBuildElement builder = new SubModuleBuildElement();
            builder.BuildInstance = false;
            builder.Target = BuildTarget.ScopeResult;

            builder.Type = "XMLCrlLanTest.ClientDBobj, XMLCrlLanTest"; //目标类型

            builder.AddStep(GetParamValueElement());

            builder.AddStep(new ModuleMethodCallElement("ToText",
               new TypeValueElement[] {
                        new TypeValueElement { Type ="object", Value = "$StepSwap" }
                  }
            ));

            item.Builder = builder;

            return item;
        }

        [TestMethod]
        public void StringExpressItemTest()
        {
            StepBasedExpressionItem<string> item = TempStrGetExpression();
            using (ModuleRunScope scope = new ModuleRunScope())
            {
                string ret = item.Invoke(scope);
                Exception error = scope.LastError;
                Assert.IsTrue(ret == "1");
            }

            XmlDocument xDoc = item.GetXmlDoc(true);
            string expStr = item.GetXmlDocString(true);

            StepBasedExpressionItem<string> restore = xDoc.GetObject<StepBasedExpressionItem<string>>();
            string xmlStr2 = restore.GetXmlDocString(true);

            Assert.AreEqual(expStr.XmlUtf8BOMClear(), xmlStr2.XmlUtf8BOMClear());
        }

        [TestMethod]
        public void ModuleFieldElementTest()
        {
            string method1 = "XMLCrlLanTest.TestClass.TestAction, XMLCrlLanTest";
            string method2 = "DBHR.WinFormUnity.SPRun.TestMethod2, DBHR.WinFormUnity";
            ModuleFieldElement ml = new ModuleFieldElement
            {
                Name = "ActionTest",
                ValueType = new TypeValueElement { Type = "System.Action<string,string>", Value = method2 },
                Flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.SetField

            };

            SPRun sp = new SPRun();
            using (ModuleRunScope scope = new ModuleRunScope())
            {
                ml.InvokeInScope(sp.GetType(), sp, scope);
            }

            SPRun.ActionTest?.Invoke("Test public static Field", "Test");

            /*
             ﻿<?xml version="1.0" encoding="utf-8"?>
             <Binding name="ActionTest" flags="Static|Public|SetField">
	                <ValueType type="System.Action&lt;System.String, System.String&gt;" value="DBHR.WinFormUnity.SPRun.TestMethod2, DBHR.WinFormUnity" />
             </Binding>
            */
            string xmlStr = ml.GetXmlDocString(true);

            ModuleFieldElement restore = ml.GetXmlDoc(true).GetObject<ModuleFieldElement>();
            Assert.AreEqual(restore.Name, ml.Name);

        }

        ModuleBranch getBranchTest(bool sharedMoudle = false)
        {
            ModuleBranch branch = new ModuleBranch
            {

            };

            //if (sharedMoudle)
            //    branch = ModuleBranch.GetScopeSharedModule("");

            ModuleWhen myWhen = new ModuleWhen();

            ConditionalElement match = new ConditionalElement();
            match.AddItem(new StringConditionItem
            {
                ExpressionLeft = TempStrGetExpression(),
                ComparisonMode = StringComparison.InvariantCultureIgnoreCase,
                BooleanCompareDelegate = "XmlClrLan.ConditionalElement.StringNotEqual, XmlClrLan",
                ExpressionRight = (StringExpressItem)"1",
            });

            myWhen.Match = match;
            myWhen.AddStep(new ModuleMethodCallElement
            {
                MethodName = "AddMainType",
                Parameters = new ModuleConstructorElement
                {
                    Arguments = new TypeValueElement[]
                    {
                        new TypeValueElement {  Type="System.Type", Value = "DBHR.WinFormUnity.TestModule2, DBHR.WinFormUnity"}
                    }

                }
            });
            branch.Conditions = new ModuleWhen[] { myWhen, myWhen };

            //Else
            SubModuleBuildElement elseModule = new SubModuleBuildElement
            {
            };
            elseModule.AddStep(new ModuleMethodCallElement
            {
                MethodName = "AddDetailType",
                Parameters = new ModuleConstructorElement
                {
                    Arguments = new TypeValueElement[]
                    {
                        new TypeValueElement {  Type="System.Type", Value = "DBHR.WinFormUnity.TestModule1, DBHR.WinFormUnity"},
                        new TypeValueElement {  Type="System.Type", Value = "DBHR.WinFormUnity.TestModule2, DBHR.WinFormUnity"}
                    }

                }
            });
            branch.Else = elseModule;

            return branch;
        }

        [TestMethod]
        public void ModuleBranchTest()
        {
            BaseEditCalenderForm frm = new BaseEditCalenderForm();
            ModuleBranch branch = getBranchTest();
            //using (ModuleRunScope scope = new ModuleRunScope())
            //{
            //    branch.InvokeInScope(frm.GetType(), frm, scope);
            //}
            string xmlStr = branch.GetXmlDocString(true);
            ModuleBranch restore = branch.GetXmlDoc(true).GetObject<ModuleBranch>();
            Assert.AreEqual(restore.Conditions.Length, branch.Conditions.Length);
        }

        [TestMethod]
        public void CompositeModuleBuildTest()
        {
            SubModuleBuildElement wholeModule = new SubModuleBuildElement
            {
                BuildInstance = true,
                ModuleId = "KQ0430",
                Target = BuildTarget.Instance,
                Type = typeof(DBHR.ClientModule.BaseEditCalenderForm).GetNoVersionTypeName()
            };

            ModuleBranch branch = getBranchTest();
            #region 修正作为子集的数据设置

            branch.Else.BuildInstance = false;
            branch.Else.ModuleId = null;
            branch.Else.Type = typeof(ModuleBuildStepElement).GetNoVersionTypeName();
            branch.Else.Target = BuildTarget.ScopeResult;
            #endregion

            wholeModule.AddStep(branch);
            wholeModule.AddStep(new ModuleMethodCallElement
            {
                MethodName = "SetCalenderObjType",
                Parameters = new ModuleConstructorElement
                {
                    Arguments = new TypeValueElement[]
                     {
                          new TypeValueElement {  Type="System.Type", Value = "DBHR.WinFormUnity.TestModule1, DBHR.WinFormUnity"}
                     }
                }
            });

            wholeModule.AddStep(new ModuleMethodCallElement { MethodName = "DoCreateFormControls" });
            wholeModule.AddStep(new ModulePropertyElement { Name = "MdiParent", Value = "$mainForm" });
            wholeModule.AddStep(new ModuleMethodCallElement { MethodName = "Show" });


            string xmlStr = wholeModule.GetXmlDocString(true);
            SubModuleBuildElement restore = wholeModule.GetXmlDoc(true).GetObject<SubModuleBuildElement>();
            Assert.AreEqual(restore.ModuleId, wholeModule.ModuleId);

            using (Form container = new Form1())
            {
                using (ModuleRunScope scope = new ModuleRunScope())
                {
                    scope.SetVariable("mainForm", container);
                    wholeModule.InvokeInScope(scope);
                    if (scope.LastError != null)
                    {
                        throw scope.LastError;
                    }
                }
            }
        }

        [TestMethod]
        public void ModuleAnyMatchTest()
        {
            using (ModuleRunScope scope = new ModuleRunScope())
            {
                string[] arrTest = new string[] { "1", "2", "3", "4", "5" };
                scope.SetVariable("$Arr", arrTest);

                ModuleAnyMatch anyMatch = new ModuleAnyMatch
                {
                    BuildInstance = false,
                    Target = BuildTarget.ScopeResult,
                    Type = typeof(System.Diagnostics.Trace).AssemblyQualifiedName,
                    ScopeIEnumerableInstance = "$Arr"
                };

                anyMatch.AddStep(new ModuleMethodCallElement
                {
                    MethodName = "WriteLine",
                    Parameters = new ModuleConstructorElement
                    {
                        Arguments = new TypeValueElement[]
                       {
                          new TypeValueElement {  Type="System.String", Value = "$StepSwap"}
                       }
                    }
                });
                anyMatch.AddStep(new ExpressionReturnElement());

                ConditionalElement when = new ConditionalElement();
                when.AddItem(new InstanceIsTypeConditionItem { ScopeInstanceKey = "$StepSwap", CompareType = "string" });
                when.AddItem(new StringConditionItem
                {
                    ComparisonMode = StringComparison.InvariantCultureIgnoreCase,
                    BooleanCompareDelegate = "XmlClrLan.ConditionalElement.StringNotEqual",
                    Logic = LogicExpression.AND,
                    ExpressionLeft = (StringExpressItem)"3",
                    ExpressionRight = (StringExpressItem)"$StepSwap",
                });
                anyMatch.Match = when;

                anyMatch.InvokeInScope(scope);

                if (scope.LastError != null)
                    throw scope.LastError;


                XmlDocument xDoc = anyMatch.GetXmlDoc(true);
                ModuleAnyMatch restore = xDoc.GetObject<ModuleAnyMatch>();

                Assert.AreEqual(restore.ModuleId, anyMatch.ModuleId);
            }
        }


        [TestMethod]
        public void IndirectTypeValueElementTest()
        {
            IndirectTypeValueElement itv = new IndirectTypeValueElement
            {
                Value = "FuncString",
                StaticField = true,
                Type = "XMLCrlLanTest.TestClass, XMLCrlLanTest"
            };

            itv.AddStep(new ModuleMethodCallElement
            {
                MethodName = "Instance",
                StaticType = "XMLCrlLanTest.TestClass2, XMLCrlLanTest"
            });

            itv.AddStep(new InstanceMethodGetElement
            {
                MethodName = "TestDelegate",
                OriginType = InstanceOrigin.Context,
                MethodDelegatePattern = typeof(TestClass.MyFuncString).GetNoVersionTypeName(),
            });

            using (ModuleRunScope scope = new ModuleRunScope())
            {
                object testObj = itv.GetObjectValue(scope);
            }

            XmlDocument xDoc = itv.GetXmlDoc(true);
            IndirectTypeValueElement restore = xDoc.GetObject<IndirectTypeValueElement>();

        }

    }
}
