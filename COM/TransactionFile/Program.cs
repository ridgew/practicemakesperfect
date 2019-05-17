using System;
using System.Transactions;

namespace KtmIntegration
{

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                using (TransactionScope transcope = new TransactionScope())
                {
                    Console.WriteLine("输入要删除的文件");
                    string path = Console.ReadLine();
                    Microsoft.KtmIntegration.TransactedFile.Delete(path);
                    Console.WriteLine("是否提交事务处理？");
                    if (Console.ReadLine() == "y")
                        transcope.Complete();
                    else
                        Transaction.Current.Rollback();
                }
            }
            catch (Exception err) { Console.WriteLine(err); }
        }
    }
}
