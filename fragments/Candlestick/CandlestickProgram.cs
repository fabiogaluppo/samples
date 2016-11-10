//Sample provided by Fabio Galuppo 
//November 2016 

//compile:
//csc CandlestickProgram.cs

using System;
using System.Linq;

namespace Candlestick
{
    class Program
    {
        static int MaxProfit(int[] L, int[] H)
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

        static int MaxProfitLINQ(int[] L, int[] H)
        {
            if (L.Length != H.Length)
                throw new RankException("L and H arrays with different sizes");

            var query = Enumerable.Range(0, L.Length)
                                  .SelectMany(i => Enumerable.Range(i + 1, H.Length - i - 1)
                                                             .Select(j => Tuple.Create(i, j)))
                                  .OrderByDescending(x => H[x.Item2] - L[x.Item1]);

            var result = query.First();

            int l = result.Item1;
            int h = result.Item2;

            if (H[h] - L[l] <= 0)
                return 0;
            return H[h] - L[l];
        }

        static int MaxProfitLinear(int[] L, int[] H)
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

        static Func<int[], int[], int> MaxProfitFunc; 

        static void Test1()
        {
            //lows
            int[] L = { 10, 9, 10, 6, 8, 5, 8, 10, 13, 3, 12, 10, 15, 14, 11, 12,
                6, 8, 11, 14, 12, 14, 8, 7, 11, 7, 9, 17, 7, 8  };
            //highs
            int[] H = { 16, 17, 13, 15, 14, 8, 12, 14, 20, 6, 19, 16, 18, 23, 22,
                21, 16, 14, 15, 18, 17, 18, 16, 15, 20, 16, 18, 20, 20, 15 };

            Console.WriteLine("Max profit is $" + MaxProfitFunc(L, H));
        }
        static void Test2()
        {
            //lows
            int[] L = { 10, 9, 3, 6, 8, 5, 8, 10, 13, 3, 12, 10, 15, 14, 11, 12,
                6, 8, 11, 14, 12, 14, 8, 7, 11, 7, 9, 17, 7, 8  };
            //highs
            int[] H = { 16, 17, 13, 50, 14, 8, 12, 14, 20, 6, 19, 16, 18, 23, 22,
                21, 16, 14, 15, 18, 17, 18, 16, 15, 20, 16, 18, 20, 20, 15 };

            Console.WriteLine("Max profit is $" + MaxProfitFunc(L, H));
        }

        static void Test3()
        {
            //lows
            int[] L = { 10, 9, 3, 6, 1, 5, 8, 10, 13, 3, 12, 10, 15, 14, 11, 12,
                6, 8, 11, 14, 12, 14, 8, 7, 11, 7, 9, 17, 7, 8  };
            //highs
            int[] H = { 16, 17, 13, 50, 14, 8, 12, 14, 20, 6, 19, 16, 49, 23, 22,
                21, 16, 14, 15, 18, 17, 18, 16, 15, 20, 16, 18, 20, 20, 15 };

            Console.WriteLine("Max profit is $" + MaxProfitFunc(L, H));
        }

        static void Test4()
        {
            //lows
            int[] L = { 10, 9, 1, 6, 3, 5, 8, 10, 13, 3, 12, 10, 15, 14, 11, 12,
                6, 8, 11, 14, 12, 14, 8, 7, 11, 7, 9, 17, 7, 8  };
            //highs
            int[] H = { 16, 17, 13, 11, 14, 8, 12, 14, 49, 6, 19, 16, 49, 23, 22,
                21, 16, 14, 15, 18, 17, 18, 16, 15, 20, 16, 18, 20, 50, 15 };

            Console.WriteLine("Max profit is $" + MaxProfitFunc(L, H));
        }

        static void Test5()
        {
            //lows
            int[] L = { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
            //highs
            int[] H = { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };

            Console.WriteLine("Max profit is $" + MaxProfitFunc(L, H));
        }

        static void Test6()
        {
            //lows
            int[] L = { 4, 3, 2, 1 };
            //highs
            int[] H = { 5, 4, 3, 2 };

            Console.WriteLine("Max profit is $" + MaxProfitFunc(L, H));
        }

        static void Test7()
        {
            //permutations of {1,2,3,4}
            //https://www.wolframalpha.com/input/?i=permutations+of+%7B1,2,3,4%7D

            int[][] Ls = 
            { 
                new int[]{1, 2, 3, 4}, new int[]{1, 2, 4, 3}, new int[]{1, 3, 2, 4}, new int[]{1, 3, 4, 2}, new int[]{1, 4, 2, 3}, new int[]{1, 4, 3, 2},
                new int[]{2, 1, 3, 4}, new int[]{2, 1, 4, 3}, new int[]{2, 3, 1, 4}, new int[]{2, 3, 4, 1}, new int[]{2, 4, 1, 3}, new int[]{2, 4, 3, 1},
                new int[]{3, 1, 2, 4}, new int[]{3, 1, 4, 2}, new int[]{3, 2, 1, 4}, new int[]{3, 2, 4, 1}, new int[]{3, 4, 1, 2}, new int[]{3, 4, 2, 1},
                new int[]{4, 1, 2, 3}, new int[]{4, 1, 3, 2}, new int[]{4, 2, 1, 3}, new int[]{4, 2, 3, 1}, new int[]{4, 3, 1, 2}, new int[]{4, 3, 2, 1}
            };

            //permutations of {2,3,4,5}
            //https://www.wolframalpha.com/input/?i=permutations+of+%7B2,3,4,5%7D

            int[][] Hs =
            {
                new int[]{2, 3, 4, 5}, new int[]{2, 3, 5, 4}, new int[]{2, 4, 3, 5}, new int[]{2, 4, 5, 3}, new int[]{2, 5, 3, 4}, new int[]{2, 5, 4, 3},
                new int[]{3, 2, 4, 5}, new int[]{3, 2, 5, 4}, new int[]{3, 4, 2, 5}, new int[]{3, 4, 5, 2}, new int[]{3, 5, 2, 4}, new int[]{3, 5, 4, 2},
                new int[]{4, 2, 3, 5}, new int[]{4, 2, 5, 3}, new int[]{4, 3, 2, 5}, new int[]{4, 3, 5, 2}, new int[]{4, 5, 2, 3}, new int[]{4, 5, 3, 2},
                new int[]{5, 2, 3, 4}, new int[]{5, 2, 4, 3}, new int[]{5, 3, 2, 4}, new int[]{5, 3, 4, 2}, new int[]{5, 4, 2, 3}, new int[]{5, 4, 3, 2}
            };

            for (int i = 0; i < Ls.Length; ++i)
            {
                int[] L = Ls[i];
                int[] H = Hs[i];
                Console.WriteLine("Max profit is $" + MaxProfitFunc(L, H));
            }
        }

        static int PerfTest1()
        {
            //lows
            int[] L = { 10, 9, 3, 6, 8, 5, 8, 10, 13, 3, 12, 10, 15, 14, 11, 12,
                6, 8, 11, 14, 12, 14, 8, 7, 11, 7, 9, 17, 7, 8  };
            //highs
            int[] H = { 16, 17, 13, 50, 14, 8, 12, 14, 20, 6, 19, 16, 18, 23, 22,
                21, 16, 14, 15, 18, 17, 18, 16, 15, 20, 16, 18, 20, 20, 15 };

            return MaxProfitFunc(L, H);
        }

        static void Main(string[] args)
        {
            //MaxProfitFunc = MaxProfit;
            //MaxProfitFunc = MaxProfitLINQ;
            MaxProfitFunc = MaxProfitLinear;

            Test1();
            Test2();
            Test3();
            Test4();
            Test5();
            Test6();
            Test7();

            Console.WriteLine();

            //microbenchmark

            //warm up
            for (int i = 0; i < 10; ++i)
            {
                MaxProfitFunc = MaxProfit;
                Console.Write(PerfTest1());
                MaxProfitFunc = MaxProfitLINQ;
                Console.Write(PerfTest1());
                MaxProfitFunc = MaxProfitLinear;
                Console.Write(PerfTest1());
            }

            Console.WriteLine();
            Console.WriteLine();

            //measure
            int result;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            MaxProfitFunc = MaxProfit;
            result = PerfTest1();
            long bf = sw.ElapsedTicks;
            Console.WriteLine("Brute force = {0,4} ticks - result = {1}", bf, result);
            sw.Restart();
            MaxProfitFunc = MaxProfitLINQ;
            result = PerfTest1();
            long linq = sw.ElapsedTicks;
            Console.WriteLine("LINQ        = {0,4} ticks - result = {1}", linq, result);
            sw.Restart();
            MaxProfitFunc = MaxProfitLinear;
            long linear = sw.ElapsedTicks;
            result = PerfTest1();
            Console.WriteLine("Linear      = {0,4} ticks - result = {1}", linear, result);
        }
    }
}
