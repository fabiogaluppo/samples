//Sample provided by Fabio Galuppo
//August 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Threading.Tasks;

using SupportLibrary;

namespace _Parallel
{
    public static class StructuredParallelismProgram
    {
        public static void Run()
        {
            RunStructuredParallelismForEach();
            RunParallelForEach();
        }
        
        private static void RunParallelForEach()
        {
            #region "Test"
            #region "evens and odds"
            int SIZE = 8 * 1000000;
            var xs = RandomGenerator.AsParallel().GetRandomizedArray(SIZE, 1, 2);

            int evens = 0, odds = 0;
            InstrumentedOperation.Test(() =>

                Parallel.ForEach(xs, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount },
                i =>
                {

                    if (i % 2 == 0) Interlocked.Increment(ref evens); else Interlocked.Increment(ref odds);

                }), "evens and odds (parallel)");
            Console.WriteLine("\tevens = {0} and odds = {1}", evens, odds);

            evens = 0; odds = 0;
            InstrumentedOperation.Test(() =>
            {

                foreach (var x in xs)
                    if (x % 2 == 0) ++evens; else ++odds;

            }, "evens and odds (sequential)");
            Console.WriteLine("\tevens = {0} and odds = {1}", evens, odds);
            #endregion

            #region "hashing"
            var cq = new System.Collections.Concurrent.ConcurrentQueue<string>();
            InstrumentedOperation.Test(() =>

                Parallel.ForEach(xs, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount },
                i =>
                {
                    cq.Enqueue(Encoding.Unicode.GetBytes(i.ToString()).ToMD5Hash());

                }), "md5 hash (parallel)");

            var q = new Queue<string>();
            InstrumentedOperation.Test(() =>
            {
                foreach (var x in xs)
                    q.Enqueue(Encoding.Unicode.GetBytes(x.ToString()).ToMD5Hash());

            }, "md5 hash (sequential)");
            #endregion
            #endregion
        }

        private static void RunStructuredParallelismForEach()
        {
            #region "Test"
            #region "evens and odds"
            int SIZE = 8 * 1000000;
            var xs = RandomGenerator.AsParallel().GetRandomizedArray(SIZE, 1, 2);
            
            int evens = 0, odds = 0;
            InstrumentedOperation.Test(() =>

                StructuredParallelism.ForEach(xs, i =>
                {
                    
                    if (i % 2 == 0) Interlocked.Increment(ref evens); else Interlocked.Increment(ref odds);

                }, Environment.ProcessorCount), "evens and odds (parallel)");
            Console.WriteLine("\tevens = {0} and odds = {1}", evens, odds);

            evens = 0; odds = 0;
            InstrumentedOperation.Test(() =>
                {
                    
                    foreach(var x in xs)
                        if (x % 2 == 0) ++evens; else ++odds;

                }, "evens and odds (sequential)");
            Console.WriteLine("\tevens = {0} and odds = {1}", evens, odds);
            #endregion

            #region "hashing"
            var cq = new System.Collections.Concurrent.ConcurrentQueue<string>();
            InstrumentedOperation.Test(() =>

                StructuredParallelism.ForEach(xs, i =>
                {
                    cq.Enqueue(Encoding.Unicode.GetBytes(i.ToString()).ToMD5Hash());

                }, Environment.ProcessorCount), "md5 hash (parallel)");
            
            var q = new Queue<string>();
            InstrumentedOperation.Test(() =>
            {
                foreach (var x in xs)
                    q.Enqueue(Encoding.Unicode.GetBytes(x.ToString()).ToMD5Hash());

            }, "md5 hash (sequential)");
            #endregion
            #endregion
        }
    }

    public static class StructuredParallelism
    {
        public static void ForEach<T>(T[] xs, Action<T> action, int degreeOfParallelism)
        {
            For(0, xs.Length, i => action(xs[i]), degreeOfParallelism);
        }

        public static void For(int offset, int count, Action<int> action, int degreeOfParallelism)
        {
            //TODO: assert arguments...

            int size = (count - offset) / degreeOfParallelism;
            CountdownEvent latch = new CountdownEvent(degreeOfParallelism);

            List<Exception> exceptions = new List<Exception>(degreeOfParallelism);
            CancellationTokenSource cts = new CancellationTokenSource();
            
            for (int i = 0; i < degreeOfParallelism; ++i)
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        int p = (int)state;
                        int start  = offset + p * size, 
                            finish = p == degreeOfParallelism - 1 ? count : start + size;
                        for (int j = start; j < finish; ++j)
                            if (!cts.IsCancellationRequested) 
                                action(j);
                    }
                    catch(Exception e)
                    {
                        lock(exceptions) exceptions.Add(e);
                        cts.Cancel();
                    }
                    
                    latch.Signal();                    
                }, i);
            }

            latch.Wait();
            cts.Dispose();

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions.ToArray());
        }
    }
}
