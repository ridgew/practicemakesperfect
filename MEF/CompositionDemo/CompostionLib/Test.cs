using CompositionInterface;
using System;
using System.ComponentModel.Composition;

namespace CompostionLib
{
    //Export出去的类型和名称都要和Import标注的属性匹配，类型可以写ITest, 也可以写Test
    [Export("wakaka", typeof(ITest))]
    public class Test : ITest
    {
        public void show()
        {
            Console.WriteLine("OK from " + typeof(Test).FullName);
        }
    }
}
