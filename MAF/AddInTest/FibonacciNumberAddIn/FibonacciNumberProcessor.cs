using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn;
using System.Threading;


namespace FibonacciAddIn
{
    [AddIn("Fibonacci Number Processor", Version = "1.0.0.0", Publisher = "Sacha Barber",
        Description = "Returns an List<int> of fiibonacci number integers within the " +
            "to/from range provided to the addin")]
    public class FibonacciNumberProcessor : AddInView.NumberProcessorAddInView
    {

        private AddInView.HostObject host;


        public static int Fibonacci(int n)
        {
            if (n == 0 || n == 1)
                return n;
            else
                return Fibonacci(n - 1) + Fibonacci(n - 2);
        }



        public override List<int> ProcessNumbers(int fromNumber, int toNumber)
        {

            List<int> results = new List<int>();

            double factor = 100 / toNumber - fromNumber;

            for (int i = fromNumber; i < toNumber; i++)
            {
                host.ReportProgress((int)(i * factor));
                results.Add(Fibonacci(i));
            }

            host.ReportProgress(100);
            return results;
  
        }


        public override void Initialize(AddInView.HostObject hostObj)
        {
            host = hostObj;
        }
    }
}
