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
    public static class ThreadsProgram
    {
        public static void Run()
        {
            _Threads.RunParallel();
            _Threads.RunParallel();
            _Threads.RunParallel();
        }
    }
    
    static class _Threads
    {
        public static void RunLimit(int minInclusive, int maxExclusive, LimitParallelism limitParalelism, string text = null)
        {
            try
            {
                limitParalelism.Enter();
                ConsoleEx.WriteLnThreaded("entered... limit count: {0} {1}", limitParalelism.Count, text);
                Thread.Sleep(new Random().Next(minInclusive, minInclusive));
            }
            finally
            {
                limitParalelism.Leave();
                ConsoleEx.WriteLnThreaded("left... limit count: {0} {1}", limitParalelism.Count, text);
            }
        }

        public static void RunParallel()
        {
            //const int SIZE = 1000000;
            const int SIZE = 100000;
            //const int SIZE = 10000;
            //const int SIZE = 1000;

            var xs = new List<string>(10240);
            for(int i = 0; i < SIZE; ++i)
            {
                xs.Add("Message #" + (i + 1));
                if (xs.Count % 10240 == 0)
                    Console.WriteLine("count = {0} capacity = {1}", xs.Count, xs.Capacity);
            }

            Console.WriteLine("count = {0} capacity = {1}", xs.Count, xs.Capacity);
            xs.Capacity = xs.Count;
            Console.WriteLine("count = {0} capacity = {1}", xs.Count, xs.Capacity);

            var ys = new List<string>();
            InstrumentedOperation.Test(() =>
                {
                    Parallel.ForEach(xs, (x) =>
                    {
                        //ys.Add(x);
                        lock (ys) ys.Add(x);
                    });
            }, "Synchronization via lock");

            Asserter.Assert(ys.Count == xs.Count, "ys.Count ({0}) == xs.Count ({1})", ys.Count, xs.Count);

            ys = new List<string>();

            InstrumentedOperation.Test(() =>
            {
                Parallel.ForEach(xs, 
                    () => new List<string>(),
                    
                    (x, state, temp) =>
                    {
                        //state.Break();
                        temp.Add(x);
                        return temp;
                    },

                    (temp) => { lock (ys) ys.AddRange(temp); 
                });
                
            }, "Synchronization via temporaries");

            Asserter.Assert(ys.Count == xs.Count, "ys.Count ({0}) == xs.Count ({1})", ys.Count, xs.Count);
        }
    }
}
