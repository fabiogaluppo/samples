//compile: csc /r:System.Core.dll /r:Microsoft.CSharp.dll /r:System.dll /r:.\bin\Lib.dll  /t:exe /out:bin\ConsoleBenchmark.exe ConsoleBenchmark.cs 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBenchmark
{
    public sealed class InstrumentedOperation
    {
        static string NormalizeString(double elapsed, int normalizer)
        {
            var elapsedNormalized = elapsed / normalizer;
            var symbol = "ms"; //millisecond
            if (elapsedNormalized < 1.0) { elapsedNormalized *= 1000; symbol = "us"; } //microseconds 
            if (elapsedNormalized < 1.0) { elapsedNormalized *= 1000; symbol = "ns"; } //nanoseconds
            if (elapsedNormalized < 1.0) { return String.Empty; }

            return String.Format("/ {0,9:0.0000} {1}", elapsedNormalized, symbol);
        }

        static internal string DoInstrumentation(Action action, string text, int normalizer, out double elapsedTotalMilliseconds)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var collectionCount = GC.CollectionCount(0);

            var sw = System.Diagnostics.Stopwatch.StartNew();
            action();
            sw.Stop();

            var elapsed = sw.Elapsed.TotalMilliseconds;
            collectionCount = GC.CollectionCount(0) - collectionCount;

            var normalizedString = normalizer > 1 ? NormalizeString(elapsed, normalizer) : String.Empty;
            elapsedTotalMilliseconds = elapsed;
            return String.Format("{0,7:N0} ms {3} (GCs={1,3}) {2}", elapsed, collectionCount, text, normalizedString);
        }

        private readonly string Result_;

        private InstrumentedOperation(Action action, string text, int normalizer)
        {
            if (null == action)
                throw new ArgumentNullException("action");

            Result_ = DoInstrumentation(action, text, normalizer, out ElapsedTotalMilliseconds_);
        }

        public override string ToString()
        {
            return Result_;
        }

        private readonly double ElapsedTotalMilliseconds_;

        public double ElapsedTotalMilliseconds
        {
            get { return ElapsedTotalMilliseconds_; }
        }

        public static InstrumentedOperation Test(Action action, string text, int normalizer = 1, bool dbgWriteLn = true)
        {
            var temp = new InstrumentedOperation(action, text, normalizer);
            return temp;
        }

        public static InstrumentedOperation Test(Action action, int normalizer = 1, bool dbgWriteLn = true)
        {
            return Test(action, String.Empty, normalizer, dbgWriteLn);
        }

        public static InstrumentedOperation<T> Test<T>(Func<T> func, string text, int normalizer = 1, bool dbgWriteLn = true)
        {
            var temp = new InstrumentedOperation<T>(func, text, normalizer);
            return temp;
        }

        public static InstrumentedOperation<T> Test<T>(Func<T> func, int normalizer = 1, bool dbgWriteLn = true)
        {
            return Test(func, String.Empty, normalizer, dbgWriteLn);
        }
    }

    public sealed class InstrumentedOperation<T>
    {
        private readonly string Result_;
        private T RetVal_;

        internal InstrumentedOperation(Func<T> func, string text, int normalizer)
        {
            if (null == func)
                throw new ArgumentNullException("func");

            Result_ = InstrumentedOperation.DoInstrumentation(() => RetVal_ = func(), text, normalizer, out ElapsedTotalMilliseconds_);
        }

        public override string ToString()
        {
            return Result_;
        }

        private readonly double ElapsedTotalMilliseconds_;

        public double ElapsedTotalMilliseconds
        {
            get { return ElapsedTotalMilliseconds_; }
        }

        public static implicit operator T(InstrumentedOperation<T> self)
        {
            return self.RetVal_;
        }
    }

    public static class EnumerableExtension
    {
        public static T Head<T>(this IEnumerable<T> xs)
        {
            return xs.First();
        }

        public static IEnumerable<T> Tail<T>(this IEnumerable<T> xs)
        {
            return xs.Skip(1);
        }

        public static IEnumerable<T> Init<T>(this IEnumerable<T> xs)
        {
            IEnumerator<T> e = xs.GetEnumerator();
            bool cont = e.MoveNext();
            while (cont)
            {
                var x = e.Current;
                cont = e.MoveNext();
                if (cont)
                    yield return x;
            }
        }

        public static string MakeString<T>(this IEnumerable<T> xs)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var x in xs) sb.Append(x);
            return sb.ToString();
        }

        public static string MakeString<T>(this IEnumerable<T> xs, string sep)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var x in xs.Init()) sb.AppendFormat("{0}{1}", x, sep);
            sb.Append(xs.Last());
            return sb.ToString();
        }

        public static string MakeString<T>(this IEnumerable<T> xs, string start, string sep, string end)
        {
            return String.Format("{0}{1}{2}", start, MakeString(xs, sep), end);
        }
    }

    class Program
    {
        static void Test(int iterations, ulong value)
        {
            Console.WriteLine("------------------------");
            Console.WriteLine("iterations = {0}, value = {1}", iterations, value);

            int SIZE = iterations;

            List<ulong> xs;
            Lib.Class1 c = new Lib.Class1();

            xs = new List<ulong>(SIZE);
            var t1 = InstrumentedOperation.Test(() =>
            {
                for (int i = 0; i < SIZE; ++i)
                    xs.AddRange(c.EnumerateSquaresImpl1(value));
            }, "EnumerateSquaresImpl1");

            Console.WriteLine(xs.Count);
            Console.WriteLine(t1);

            xs = new List<ulong>();
            var t2 = InstrumentedOperation.Test(() =>
            {
                for (int i = 0; i < SIZE; ++i)
                    xs.AddRange(c.EnumerateSquaresImpl2(value));
            }, "EnumerateSquaresImpl2");

            Console.WriteLine(xs.Count);
            Console.WriteLine(t2);

            xs = new List<ulong>(SIZE);
            var t3 = InstrumentedOperation.Test(() =>
            {
                for (int i = 0; i < SIZE; ++i)
                    xs.AddRange(c.EnumerateSquaresImpl3(value));
            }, "EnumerateSquaresImpl3");

            Console.WriteLine(xs.Count);
            Console.WriteLine(t3);

            xs = new List<ulong>(SIZE);
            var t4 = InstrumentedOperation.Test(() =>
            {
                for (int i = 0; i < SIZE; ++i)
                    xs.AddRange(c.EnumerateSquaresImpl4(value));
            }, "EnumerateSquaresImpl4");

            Console.WriteLine(xs.Count);
            Console.WriteLine(t4);

            Console.WriteLine("------------------------");
        }
        
        static void PrintSeq(IEnumerable<ulong> xs)
        {
            foreach(var x in xs)
                Console.Write(x + " ");
            Console.WriteLine();
        }

        static void PrintSeqs()
        {
            Lib.Class1 c = new Lib.Class1();

            foreach (var n in new ulong[] { UInt64.MaxValue, 42, 12297829382473034410, UInt64.MinValue })
            {
                PrintSeq(c.EnumerateSquaresImpl1(n));
                PrintSeq(c.EnumerateSquaresImpl2(n));
                PrintSeq(c.EnumerateSquaresImpl3(n));
                PrintSeq(c.EnumerateSquaresImpl4(n));
            }
        }

        static void PrintTests()
        {
            foreach (var k in new ulong[] { UInt64.MaxValue, 42, 12297829382473034410, UInt64.MinValue })
                foreach (var n in new int[] { 100, 1000, 10000, 100000, 1000000 })
                    Test(n, k);
        }

        static void Main(string[] args)
        {
            //PrintSeqs();

            PrintTests();
        }
    }
}
