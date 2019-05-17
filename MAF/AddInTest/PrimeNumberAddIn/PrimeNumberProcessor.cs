using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn;
using System.Threading;


namespace PrimeNumberAddIn
{
    [AddIn("Prime Number Processor", Version = "1.0.0.0", Publisher = "Sacha Barber",
        Description = "Returns an List<int> of prime number integers within the" +
            "to/from range provided to the addin")]
    public class PrimeNumberProcessor : AddInView.NumberProcessorAddInView 
    {

        private AddInView.HostObject host;

        public override List<int> ProcessNumbers(int fromNumber, int toNumber)
        {
            List<int> results = new List<int>();
            int[] list = new int[toNumber - fromNumber];
            double factor = 100 / toNumber - fromNumber;

            // Create an array containing all integers between the two specified numbers.
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = fromNumber;
                fromNumber += 1;
            }

            //find out the module for each item in list, divided by each d, where
            //d is < or == to sqrt(to)
            //mark composite with 1, and primes with 0 in mark array
            int maxDiv = (int)Math.Floor(Math.Sqrt(toNumber));

            int[] mark = new int[list.Length];

            for (int i = 0; i < list.Length; i++)
            {
                for (int j = 2; j <= maxDiv; j++)
                    if ((list[i] != j) && (list[i] % j == 0))
                        mark[i] = 1;

                host.ReportProgress((int)(i * factor));
            }

            //get the marked primes from original array
            for (int i = 0; i < mark.Length; i++)
                if (mark[i] == 0)
                    results.Add(list[i]);

            host.ReportProgress(100);
            return results;
        }

        public override void Initialize(AddInView.HostObject hostObj)
        {
            host = hostObj;
        }
    }
}
