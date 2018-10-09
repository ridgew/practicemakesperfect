using System;
using System.Reflection;
using System.Diagnostics.Tracing;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlClrLan;

namespace XMLCrlLanTest
{
    [TestClass]
    public class CodeSnippetTest
    {
        [TestMethod]
        public void TestBindingFlags()
        {
            string bindingFlagSet = "Instance";
            BindingFlags flag = bindingFlagSet.AsBindingFlags();
            System.Diagnostics.Trace.WriteLine(flag);
        }
    }
}
