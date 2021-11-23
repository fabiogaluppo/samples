//Sample provided by Fabio Galuppo
//November 2021

using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace PerformancePlayground
{
    [MemoryDiagnoser]
    public class BenchmarkProgram
    {
        [Benchmark]
        [Arguments(100)]
        [Arguments(1000)]
        [Arguments(10000)]
        [Arguments(100000)]
        [Arguments(1000000)]
        public void FizzBuzz1(int N)
        {
            FizzBuzzPartitions.FizzBuzz1(Enumerable.Range(1, N));
        }

        [Benchmark]
        [Arguments(100)]
        [Arguments(1000)]
        [Arguments(10000)]
        [Arguments(100000)]
        [Arguments(1000000)]
        public void FizzBuzz2(int N)
        {
            FizzBuzzPartitions.FizzBuzz2(Enumerable.Range(1, N));
        }

        [Benchmark]
        [Arguments(100)]
        [Arguments(1000)]
        [Arguments(10000)]
        [Arguments(100000)]
        [Arguments(1000000)]
        public void FizzBuzz3(int N)
        {
            FizzBuzzPartitions.FizzBuzz3(Enumerable.Range(1, N));
        }

        [Benchmark]
        [Arguments(100)]
        [Arguments(1000)]
        [Arguments(10000)]
        [Arguments(100000)]
        [Arguments(1000000)]
        public void FizzBuzz4(int N)
        {
            FizzBuzzPartitions.FizzBuzz4(Enumerable.Range(1, N));
        }

        public static void Run(string[] args)
        {
            var summary = BenchmarkRunner.Run<BenchmarkProgram>();
        }
    }
}
