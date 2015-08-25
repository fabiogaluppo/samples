//Sample provided by Fabio Galuppo 
//August 2015

//compile: csc /r:System.Core.dll /r:Microsoft.CSharp.dll /r:System.dll /t:exe /out:bin\Program.exe Program.cs
//run: .\bin\Program.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

#region "priority_queue"
public abstract class PQ<T> where T : IComparable<T>
{
    protected sealed class KeyValue
    {
        public readonly int Key;
        public readonly T Value;

        public KeyValue(int key, T value)
        {
            Key = key;
            Value = value;
        }
    }

    private SortedDictionary<KeyValue, KeyValue> PQ_;
    
    protected PQ(IComparer<KeyValue> comparer)
    {
        PQ_ = new SortedDictionary<KeyValue, KeyValue>(comparer);
    }

    private int Key_;

    public void Push(T x) 
    {
        var x_ = new KeyValue(Key_++, x);
        PQ_.Add(x_, x_); 
    }

    public int Size { get { return PQ_.Count; } }

    public bool IsEmpty { get { return PQ_.Count == 0; } }

    public void Pop() { PQ_.Remove(PQ_.First().Key); }

    public T Top { get { return PQ_.First().Value.Value; } }

    public static MinPQ<T> CreateMinPQ() { return new MinPQ<T>(); }

    public static MaxPQ<T> CreateMaxPQ() { return new MaxPQ<T>(); }
}

public sealed class MinPQ<T> : PQ<T> where T : IComparable<T>
{
    private sealed class GreaterThanComparer : IComparer<KeyValue>
    {
        public int Compare(KeyValue x, KeyValue y)
        {
            int res = x.Value.CompareTo(y.Value);
            if (res != 0)
                return res;
            return x.Key.CompareTo(y.Key);
        }
    }

    private static GreaterThanComparer GreaterThan = new GreaterThanComparer();

    public MinPQ() : base(GreaterThan) {}
}

public sealed class MaxPQ<T> : PQ<T> where T : IComparable<T>
{
    private sealed class LessThanComparer : IComparer<KeyValue>
    {
        public int Compare(KeyValue x, KeyValue y)
        {
            int res = y.Value.CompareTo(x.Value);
            if (res != 0)
                return res;
            return x.Key.CompareTo(y.Key);
        }
    }

    private static LessThanComparer LessThan = new LessThanComparer();

    public MaxPQ() : base(LessThan) {}
}

#endregion

class Program
{
    static IEnumerable<int> NaiveMedianMaintenance(IEnumerable<int> xs)
    {
        List<int> ys = new List<int>();
        
        int y = 0;
        foreach (var x in xs) //lg(H(N)) + N + 2 N where H is hyperfactorial function
        {
            ys.Add(x); //N
            ys.Sort(); //lg(H(N))

            //2 N [1 (if) + 1 (array access) or 1 (else) + 1 (array access)]
            if (0x1 == (ys.Count & 0x1)) //is odd?
            {
                y = ys[ys.Count / 2];
            }
            else
            {
                y = ys[(ys.Count - 1) / 2];
            }

            yield return y;

            //DEBUG
            //Console.WriteLine("Median of {0,4} = {1,4}", ys.Count, y);
        }
    }

    static T ExtractMin<T>(MinPQ<T> pq) where T : IComparable<T>
    {
        T x = pq.Top;
        pq.Pop();
        return x;
    }

    static T ExtractMax<T>(MaxPQ<T> pq) where T : IComparable<T>
    {
        T x = pq.Top;
        pq.Pop();
        return x;
    }

    static IEnumerable<int> MedianMaintenance(IEnumerable<int> xs)
    {
        MaxPQ<int> lowValues = PQ<int>.CreateMaxPQ();
        MinPQ<int> highValues = PQ<int>.CreateMinPQ();

        int y = 0;
        foreach (var x in xs) //lg(N!) + 4/3 lg (N!) == 7/3 lg (N!)
        {
            if (lowValues.Size > 0) //lg(N!)
            {
                if (x > lowValues.Top)
                {
                    highValues.Push(x); //lg N
                }
                else
                {
                    lowValues.Push(x); //lg N
                }
            }
            else
            {
                lowValues.Push(x); //lg N
            }

            //TODO: Not a simple analysis, depends on order of data in xs
            //Draft: Suppose 1/3 per branch and 2 lg N Push and Extract, the expected value would be 4/3 lg (N!)
            if (lowValues.Size > highValues.Size + 1)
            {
                highValues.Push(ExtractMax(lowValues)); //2 lg N (Push + Extract)
            }
            else if (highValues.Size > lowValues.Size)
            {
                lowValues.Push(ExtractMin(highValues)); //2 lg N (Push + Extract)
            }
            //else { } //0


            y = lowValues.Top;
            yield return y;

            //DEBUG
            //Console.WriteLine("Median of {0,4} = {1,4}", lowValues.Size + highValues.Size, y);
        }
    }
    
    static Random Rnd = new Random(); 
    
    static IEnumerable<int> RandomRange(int N)
    {
        for (int i = 0; i < N; ++i)
            yield return Rnd.Next(1, 1000000);
    }  
    
    static void Test(int N)
    {
        Console.WriteLine("------------------------");
        Console.WriteLine("N = " + N);
        
        var xs = RandomRange(N).ToList();
        
        List<int> ys = new List<int>();
        var t1 = InstrumentedOperation.Test(() => { 
            ys.AddRange(NaiveMedianMaintenance(xs)); 
        }, "Naive Median Maintenance");
        Console.Write(ys[0]);
        for (int i = 1; i < 10; ++i) Console.Write(" " + ys[i]);
        Console.WriteLine();
        Console.WriteLine(t1);

        ys = new List<int>();
        var t2 = InstrumentedOperation.Test(() => {
             ys.AddRange(MedianMaintenance(xs)); 
        }, "Median Maintenance");
        Console.Write(ys[0]);
        for (int i = 1; i < 10; ++i) Console.Write(" " + ys[i]);
        Console.WriteLine();
        Console.WriteLine(t2);
        
        Console.WriteLine("------------------------");
    }
    
    static void Main(string[] args)
    {
        foreach (var n in new int[] { 100, 100, 1000, 10000, 100000 })
            Test(n);
    }
}
