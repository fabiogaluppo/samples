//Sample provided by Fabio Galuppo
//November 2021

using System;
using System.Linq;

namespace PerformancePlayground
{
    public class Program
    {
        /*
         * FizzBuzz: ... values in ascending order 
         * Fizz: ...
         * Buzz: ...
         * Numbers: ...
         * 
         * Function signature: ValueTuple<string, ReadOnlyMemory<int>>[] FizzBuzz(IEnumerable<int> numbers)
         */

        static void Display(ValueTuple<string, ReadOnlyMemory<int>>[] xs)
        {
            foreach (var x in xs)
            {
                ReadOnlySpan<int> ys = x.Item2.Span;
                Console.Write($"{x.Item1} ({ys.Length}): ");
                for (int i = 0; i < ys.Length; ++i)
                    Console.Write($"{ys[i]} ");
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        static void Main(string[] args)
        {
            //Candlestick.Run(args);
            //return;

            //Histogram.Run(args);
            //return;

            //BenchmarkProgram.Run(args);
            //return;

            Display(FizzBuzzPartitions.FizzBuzz1(Enumerable.Range(1, 100)));
            Display(FizzBuzzPartitions.FizzBuzz2(Enumerable.Range(1, 100)));
            Display(FizzBuzzPartitions.FizzBuzz3(Enumerable.Range(1, 100)));
            Display(FizzBuzzPartitions.FizzBuzz4(Enumerable.Range(1, 100)));
            var random = new Random();
            var numbers = Enumerable.Range(1, 100).Select(_ => random.Next(1, 200)).ToArray();
            Display(FizzBuzzPartitions.FizzBuzz1(numbers));
            Display(FizzBuzzPartitions.FizzBuzz2(numbers));
            Display(FizzBuzzPartitions.FizzBuzz3(numbers));
            Display(FizzBuzzPartitions.FizzBuzz4(numbers));
            return;

            Partitioner.Test();
        }
    }
}
