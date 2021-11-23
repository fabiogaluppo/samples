//Sample provided by Fabio Galuppo
//November 2021

using System;
using System.Collections.Generic;
using System.Linq;

namespace PerformancePlayground
{
    public static class FizzBuzzPartitions
    {
        private readonly static string[] labels = new string[] { "FizzBuzz", "Fizz", "Buzz", "Numbers" };
        private const int FIZZBUZZ = 0;
        private const int FIZZ = 1;
        private const int BUZZ = 2;
        private const int NUMBERS = 3;

        #region "LINQ"
        public static ValueTuple<string, ReadOnlyMemory<int>>[] FizzBuzz1(IEnumerable<int> numbers)
        {
            var result =
                numbers
                    .ToLookup(x =>
                    {
                        bool fizz = x % 3 == 0, buzz = x % 5 == 0;
                        if (fizz && buzz) return FIZZBUZZ;
                        if (fizz) return FIZZ;
                        if (buzz) return BUZZ;
                        return NUMBERS;
                    }, x => x);
            return result.OrderBy(g => g.Key)
                         .Select(g => ValueTuple.Create(labels[g.Key], new ReadOnlyMemory<int>(g.OrderBy(i => i).ToArray())))
                         .ToArray();
        }
        #endregion

        #region "Algorithmic approach"
        //Binary Inplace Partition
        private static int Partition(Span<int> xs, Predicate<int> predicate)
        {
            int first = 0;
            int last = xs.Length;
            while (first != last)
            {
                while (predicate(xs[first]))
                {
                    ++first;
                    if (first == last)
                        return first;
                }
                do
                {
                    --last;
                    if (first == last)
                        return first;
                }
                while (!predicate(xs[last]));
                int temp = xs[first];
                xs[first] = xs[last];
                xs[last] = temp;
                ++first;
            }
            return first;
        }
        
        public static ValueTuple<string, ReadOnlyMemory<int>>[] FizzBuzz2(IEnumerable<int> numbers)
        {
            var arr = numbers.ToArray();
            var span = new Span<int>(arr);
            var result = new ValueTuple<string, ReadOnlyMemory<int>>[4];

            int start1 = 0;
            var span1 = span.Slice(start1, Partition(span.Slice(start1), x => x % 5 == 0 && x % 3 == 0));
            int length1 = span1.Length;
            span1.Sort();
            result[FIZZBUZZ] = ValueTuple.Create(labels[FIZZBUZZ], new ReadOnlyMemory<int>(arr, start1, length1));

            int start2 = span1.Length;
            var span2 = span.Slice(start2, Partition(span.Slice(start2), x => x % 3 == 0));
            int length2 = span2.Length;
            span2.Sort();
            result[FIZZ] = ValueTuple.Create(labels[FIZZ], new ReadOnlyMemory<int>(arr, start2, length2));

            int start3 = span1.Length + span2.Length;
            var span3 = span.Slice(start3, Partition(span.Slice(start3), x => x % 5 == 0));
            int length3 = span3.Length;
            span3.Sort();
            result[BUZZ] = ValueTuple.Create(labels[BUZZ], new ReadOnlyMemory<int>(arr, start3, length3));

            int start4 = span1.Length + span2.Length + span3.Length;
            var span4 = span.Slice(start4);
            int length4 = span4.Length;
            span4.Sort();
            result[NUMBERS] = ValueTuple.Create(labels[NUMBERS], new ReadOnlyMemory<int>(arr, start4, length4));

            return result;
        }
        #endregion

        #region "Algorithm approach + Embarrassingly parallel"
        private static System.Threading.Tasks.Task SortRange(int[] xs, int start, int length)
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                Array.Sort(xs, start, length);
            });
        }

        private static int PartitionRange(Span<int> span, int start, Predicate<int> predicate)
        {
            return span.Slice(start, Partition(span.Slice(start), predicate)).Length;
        }

        private static int PartitionRange(Span<int> span, int start)
        {
            return span.Slice(start).Length;
        }

        private static ValueTuple<string, ReadOnlyMemory<int>> PrepareResult(int[] xs, int start, int length, int label)
        {
            return ValueTuple.Create(labels[label], new ReadOnlyMemory<int>(xs, start, length));
        }

        public static ValueTuple<string, ReadOnlyMemory<int>>[] FizzBuzz3(IEnumerable<int> numbers)
        {
            var arr = numbers.ToArray();
            var span = new Span<int>(arr);
            var result = new ValueTuple<string, ReadOnlyMemory<int>>[4];

            int start1 = 0;
            int length1 = PartitionRange(span, start1, x => x % 5 == 0 && x % 3 == 0);
            var task1 = SortRange(arr, start1, length1);
            result[FIZZBUZZ] = PrepareResult(arr, start1, length1, FIZZBUZZ);

            int start2 = length1;
            int length2 = PartitionRange(span, start2, x => x % 3 == 0);
            var task2 = SortRange(arr, start2, length2);
            result[FIZZ] = PrepareResult(arr, start2, length2, FIZZ);

            int start3 = length1 + length2;
            int length3 = PartitionRange(span, start3, x => x % 5 == 0);
            var task3 = SortRange(arr, start3, length3);
            result[BUZZ] = PrepareResult(arr, start3, length3, BUZZ);

            int start4 = length1 + length2 + length3;
            int length4 = PartitionRange(span, start4);
            var task4 = SortRange(arr, start4, length4);
            result[NUMBERS] = PrepareResult(arr, start4, length4, NUMBERS);

            System.Threading.Tasks.Task.WaitAll(task1, task2, task3, task4);

            return result;
        }
        #endregion

        #region "From Concrete to Generic"        
        public static ValueTuple<string, ReadOnlyMemory<int>>[] FizzBuzz4(IEnumerable<int> numbers)
        {
            return Partitioner.Do(numbers, i => labels[i], x => x % 5 == 0 && x % 3 == 0, x => x % 3 == 0, x => x % 5 == 0);
        }
        #endregion
    }
}
