//Sample provided by Fabio Galuppo
//November 2021

using System;
using System.Collections.Generic;
using System.Linq;

namespace PerformancePlayground
{
    public static class Partitioner
    {
        private static int Partition<T>(Span<T> xs, Predicate<T> predicate)
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
                T temp = xs[first];
                xs[first] = xs[last];
                xs[last] = temp;
                ++first;
            }
            return first;
        }

        private static int PartitionRange<T>(Span<T> span, int start, Predicate<T> predicate)
        {
            return span.Slice(start, Partition(span.Slice(start), predicate)).Length;
        }

        private static int PartitionRange<T>(Span<T> span, int start)
        {
            return span.Slice(start).Length;
        }

        private static System.Threading.Tasks.Task SortRange<T>(T[] xs, int start, int length)
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                Array.Sort(xs, start, length);
            });
        }
        public static ValueTuple<TKey, ReadOnlyMemory<TValue>>[] Do<TKey, TValue>(TValue[] arr, Func<int, TKey> keySelector, params Predicate<TValue>[] predicates)
        {
            var span = new Span<TValue>(arr);
            int N = predicates.Length;
            var result = new ValueTuple<TKey, ReadOnlyMemory<TValue>>[N + 1];
            var tasks = new System.Threading.Tasks.Task[N + 1];
            int start = 0, length;
            for (int i = 0; i < N; ++i)
            {
                length = PartitionRange(span, start, predicates[i]);
                tasks[i] = SortRange(arr, start, length);
                result[i] = ValueTuple.Create(keySelector(i), new ReadOnlyMemory<TValue>(arr, start, length));
                start += length;
            }
            length = PartitionRange(span, start);
            tasks[N] = SortRange(arr, start, length);
            result[N] = ValueTuple.Create(keySelector(N), new ReadOnlyMemory<TValue>(arr, start, length));

            System.Threading.Tasks.Task.WaitAll(tasks);

            return result;
        }

        public static ValueTuple<TKey, ReadOnlyMemory<TValue>>[] Do<TKey, TValue>(IEnumerable<TValue> items, Func<int, TKey> keySelector, params Predicate<TValue>[] predicates)
        {
            return Partitioner.Do(items.ToArray(), keySelector, predicates);
        }
        internal static void Test()
        {
            var bands = new string[]
            {
                "Iron Maiden", "Metallica", "Dream Theater", "Lord", "Riverside", "Eagles",
                "Rocket Scientists", "Whitesnake", "Lunatic Soul", "Metal Church", "Rush", "Elvis Presley"
            };
            var labels = new string[]
            {
                "M", "E", "T", "A", "L", "*"
            };
            var xs = Partitioner.Do(bands,
                i => labels[i],
                x => x.StartsWith(labels[0]),
                x => x.StartsWith(labels[1]),
                x => x.StartsWith(System.Text.Encoding.ASCII.GetString(new byte[] { (byte)(System.Text.Encoding.ASCII.GetBytes(labels[2])[0] - 2) })),
                x => x.StartsWith(labels[3]),
                x => x.StartsWith(labels[4])
            );
            foreach (var x in xs)
            {
                ReadOnlySpan<string> ys = x.Item2.Span;
                Console.Write($"{x.Item1} ({ys.Length}): ");
                for (int i = 0; i < ys.Length; ++i)
                    Console.Write($"[{ys[i]}] ");
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
