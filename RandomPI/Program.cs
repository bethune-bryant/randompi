using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace RandomPI
{
    class Program
    {
        static void Main(string[] args)
        {
            int runtime = 10; if (args.Count() > 0) runtime = int.Parse(args[0]);
            int threads = Environment.ProcessorCount; if (args.Count() > 1) threads = int.Parse(args[1]);
            int seed = 31415926; if (args.Count() > 2) seed = int.Parse(args[2]);

            ConcurrentBag<int> coPrimeCounts = new ConcurrentBag<int>();
            ConcurrentBag<int> totalCounts = new ConcurrentBag<int>();
            Parallel.For(0, threads, delegate (int i)
            {
                DateTime start = DateTime.Now;

                Random rand = new Random(seed + i);

                int count = 0;
                int coPrimes = 0;
                while ((DateTime.Now - start).TotalSeconds < runtime)
                {
                    count++;
                    if (Utilities.IsCoPrime(Utilities.RandomLong(rand), Utilities.RandomLong(rand))) coPrimes++;
                }

                totalCounts.Add(count);
                coPrimeCounts.Add(coPrimes);
            });

            int totalCount = totalCounts.Sum();
            int totalCoPrimes = coPrimeCounts.Sum();

            double randomPI = Math.Sqrt((6.0 * totalCount) / totalCoPrimes);

            Console.WriteLine("Total Samples: " + totalCount);
            Console.WriteLine("Result: " + randomPI);
            Console.WriteLine("Error: " + ((Math.Abs(randomPI - Math.PI) / Math.PI) ).ToString("P"));
        }
    }

    static class Utilities
    {
        public static ulong GCD(ulong a, ulong b)
        {
            ulong Remainder;

            while (b != 0)
            {
                Remainder = a % b;
                a = b;
                b = Remainder;
            }

            return a;
        }

        public static bool IsCoPrime(ulong a, ulong b)
        {
            return GCD(a, b) == 1;
        }

        public static ulong RandomLong(Random rand)
        {
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            ulong longRand = BitConverter.ToUInt64(buf, 0);
            return longRand;
        }
    }
}
