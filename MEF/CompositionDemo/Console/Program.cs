using CompositionInterface;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

//C# System.ComponentModel.Composition中的Export和Import特性标签的简单使用。
//https://blog.csdn.net/a1037949156/article/details/79535129


//MEF 打造的插件系统
//http://www.cnblogs.com/LoveJenny/archive/2011/12/07/2278703.html

//https://docs.microsoft.com/zh-cn/dotnet/api/system.componentmodel.composition.hosting.compositioncontainer?view=netframework-4.0


namespace CompositionDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Go go = new Go();

            //获取当前执行的程序集中所有的标有特性标签的代码段
            using (var catalog = new AggregateCatalog())
            //将所有Export特性标签存放进组件容器中（其实是一个数组里面）
            using (var container = new CompositionContainer(catalog))
            {
                catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
                catalog.Catalogs.Add(new DirectoryCatalog("."));

                var types = container.GetExportedValues<ITest>("static");
                foreach (var t in types)
                {
                    Console.WriteLine("导出静态实现类:{0}", t.GetType().FullName);
                }
                //找到所传入对象中所有拥有Import特性标签的属性，并在组件容器的数组中找到与这些属性匹配的Export特性标签所标注的类，然后进行实例化并给这些属性赋值。
                //简而言之，就是找到与Import对应的Export所标注的类，并用这个类的实例来给Import所标注的属性赋值，用于解耦。

                try
                {
                    container.ComposeParts(go);
                }
                catch (CompositionException compositionException)
                {
                    Console.WriteLine(compositionException.ToString());
                }
            }

            if (go.test != null)
            {
                go.test.show();
            }
            Console.ReadLine();
        }
    }

    public class Go
    {
        [Import("wakaka")]
        public ITest test { get; set; }
    }

    public class Go3
    {
        [Export("static", typeof(ITest))]
        public static readonly ITest TestStatic = new Go2P();
    }


    public class Go2P : ITest
    {
        public void show()
        {
            ;
        }
    }

}
