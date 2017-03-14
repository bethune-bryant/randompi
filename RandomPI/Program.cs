using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace RandomPI
{
    /// <summary>
    /// Approximates PI as described here:
    /// https://www.youtube.com/watch?v=RZBhSi_PwHU
    /// Using the fact that the probability of any 2 positive integers being coprime is 6/(pi^2).
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //The number of seconds to compute samples.
            int runtime = 10; if (args.Count() > 0) runtime = int.Parse(args[0]);
            //The number of threads to run on. By default all available threads.
            int threads = Environment.ProcessorCount; if (args.Count() > 1) threads = int.Parse(args[1]);
            //The random generator seed. (pi by deafult!)
            int seed = 31415926; if (args.Count() > 2) seed = int.Parse(args[2]);

            ConcurrentBag<int> coPrimeCounts = new ConcurrentBag<int>();
            ConcurrentBag<int> totalCounts = new ConcurrentBag<int>();
            Parallel.For(0, threads, delegate (int i)
            {
                DateTime start = DateTime.Now;
                Random rand = new Random(seed + i);

                int count = 0;
                int coPrimes = 0;
                while ((DateTime.Now - start).TotalSeconds < runtime) //Find random samples for <runtime> seconds.
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
        /// <summary>
        /// Returns the greatest common divisor of two unsigned long values. 
        /// </summary>
        /// <param name="a">First Value</param>
        /// <param name="b">Second Value</param>
        /// <returns>The greatest common divisor of two unsigned long values.</returns>
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

        /// <summary>
        /// Determines whether or not two unsigned long values are coprime. 
        /// Meaning their greatest common divisor is 1.
        /// </summary>
        /// <param name="a">First Value</param>
        /// <param name="b">Second Value</param>
        /// <returns>True if the two values are coprime, false otherwise.</returns>
        public static bool IsCoPrime(ulong a, ulong b)
        {
            return GCD(a, b) == 1;
        }

        /// <summary>
        /// Uses an input random number generator to generate a random unsigned long value.
        /// </summary>
        /// <param name="rand">Random number generator.</param>
        /// <returns>Random unsigned long.</returns>
        public static ulong RandomLong(Random rand)
        {
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            ulong longRand = BitConverter.ToUInt64(buf, 0);
            return longRand;
        }
    }
}
