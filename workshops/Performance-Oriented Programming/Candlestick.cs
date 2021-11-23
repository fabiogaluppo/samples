//Sample provided by Fabio Galuppo
//November 2021

using System;
using System.Linq;

namespace PerformancePlayground
{
    public static class Candlestick
    {
        private static int MaxProfitBruteForce(int[] L, int[] H)
        {
            if (L.Length != H.Length)
                throw new RankException("L and H arrays with different sizes");

            int l = 0, h = 0;
            for (int i = 0; i < L.Length; ++i)
            {
                for (int j = i + 1; j < H.Length; ++j)
                {
                    if (H[j] - L[i] > H[h] - L[l])
                    {
                        h = j;
                        l = i;
                    }
                }
            }

            if (h == l)
                return 0;
            return H[h] - L[l];
        }

        private static int MaxProfitLinear(int[] L, int[] H)
        {
            if (L.Length != H.Length)
                throw new RankException("L and H arrays with different sizes");

            int min1 = 0, max1 = 0, min2 = min1, max2 = max1;

            for (int i = 1; i < L.Length; ++i)
            {
                if (H[i] > H[max1])
                {
                    max1 = i;
                    if (H[i] > H[max2])
                    {
                        max2 = i;
                        if (H[max1] - L[min1] < H[max2] - L[min2])
                        {
                            min1 = min2;
                            max1 = max2;
                        }
                        continue;
                    }
                }

                if (L[i] < L[min1])
                {
                    if (L[i] < L[min2])
                    {
                        min2 = i;
                        max2 = i;
                        continue;
                    }
                }

                if (H[i] > H[max2])
                {
                    max2 = i;
                    if (H[max1] - L[min1] < H[max2] - L[min2])
                    {
                        min1 = min2;
                        max1 = max2;
                    }
                }
            }

            int l = min1;
            int h = max1;

            if (h == l)
                return 0;
            return H[h] - L[l];
        }

        public static void Run(string[] args)
        {
            //lows
            int[] L = { 10, 9, 3, 6, 8, 5, 8, 10, 13, 3, 12, 10, 15, 14, 11, 12,
                6, 8, 11, 14, 12, 14, 8, 7, 11, 7, 9, 17, 7, 8  };
            //highs
            int[] H = { 16, 17, 13, 50, 14, 8, 12, 14, 20, 6, 19, 16, 18, 23, 22,
                21, 16, 14, 15, 18, 17, 18, 16, 15, 20, 16, 18, 20, 20, 15 };

            const int N_WARMUP = 5;
            const int N_MEASUREMENT = 10;

            Console.WriteLine("Stopwatch is high-resolution? {0}", System.Diagnostics.Stopwatch.IsHighResolution);
            Console.WriteLine();
            
            //warm-up
            Console.WriteLine("Warm-up");
            Console.WriteLine("-------");
            for (int i = 0; i < N_WARMUP; ++i)
            {
                Console.WriteLine($"{i + 1}:");
                Console.WriteLine("Brute force profit: {0}", MaxProfitBruteForce(L, H));
                Console.WriteLine("     Linear profit: {0}", MaxProfitLinear(L, H));
            }
            Console.WriteLine("==============================");
            Console.WriteLine();

            //measurement
            Console.WriteLine("Measurement");
            Console.WriteLine("-----------");           
            int result;
            long ticks;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var bfTicks = new long[N_MEASUREMENT];
            var lnTicks = new long[N_MEASUREMENT];
            for (int i = 0; i < N_MEASUREMENT; ++i)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Console.WriteLine($"{i + 1}:");

                sw.Restart();
                result = MaxProfitBruteForce(L, H);
                ticks = sw.ElapsedTicks;
                Console.WriteLine("Brute force = {0,4} ticks - result = {1}", ticks, result);
                bfTicks[i] = ticks;

                sw.Restart();
                result = MaxProfitLinear(L, H);
                ticks = sw.ElapsedTicks;
                Console.WriteLine("Linear      = {0,4} ticks - result = {1}", ticks, result);
                lnTicks[i] = ticks;
            }
            Console.WriteLine("==============================");
            Console.WriteLine();

            Console.WriteLine("Summary");
            Console.WriteLine("-------");
            Console.WriteLine("Brute force = {0,5:0.00} average ticks", (double)bfTicks.Sum() / N_MEASUREMENT);
            Console.WriteLine("Linear      = {0,5:0.00} average ticks", (double)lnTicks.Sum() / N_MEASUREMENT);
            Console.WriteLine("==============================");
            Console.WriteLine();
        }
    }
}
