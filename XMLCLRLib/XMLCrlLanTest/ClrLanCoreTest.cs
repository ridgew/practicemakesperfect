using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlClrLan;

namespace XMLCrlLanTest
{
    [TestClass]
    public class ClrLanCoreTest
    {
        [TestMethod]
        public void DisposableScopeItemTest()
        {
            SubModuleBuildElement myBuilder = new SubModuleBuildElement
            {
                BuildInstance = true,
                ModuleId = "sc",
                Type = "",
                Target = BuildTarget.Instance
            };

            myBuilder.AddStep(new ModuleConstructorElement
            {
                Arguments = new TypeValueElement[0]
            });

            DisposableScopeItem item = new DisposableScopeItem
            {
                Builder = myBuilder
            };

            using (ModuleRunScope scope = new ModuleRunScope())
            {
                scope.SetVariable(item.Builder.ModuleId, item);
                using (item.Invoke(scope))
                {
                    //IExpressionItem<Target>
                }
            }
        }
    }
}
