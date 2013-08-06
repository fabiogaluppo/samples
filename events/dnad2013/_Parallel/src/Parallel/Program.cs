//Sample provided by Fabio Galuppo
//August 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using SupportLibrary;

namespace _Parallel
{
    static class TestProgram
    {
        public static void RunRandomizedArray()
        {
            int[] SIZES = { 100, 1000, 10000, 100000, 1000000, 2 * 1000000, 20 * 1000000 };

            foreach (var SIZE in SIZES)
            {
                var strSize = String.Format("{0,12:#,##0}", SIZE);
                SupportLibrary.InstrumentedOperation.Test(() =>
                {
                    SupportLibrary.RandomGenerator.AsSequential().GetRandomizedArray(SIZE, 1, 1);
                }, "sequential" + strSize);

                SupportLibrary.InstrumentedOperation.Test(() =>
                {
                    SupportLibrary.RandomGenerator.AsParallel().GetRandomizedArray(SIZE, 1, 1);
                }, "parallel  " + strSize);
            }
        }

        public static void RunSpawningThreads()
        {
            ConsoleEx.WriteLnThreaded("spawning threads...");

            InstrumentedOperation.Test(() =>
            {
                var t = new Thread(() =>
                {
                    //...
                    ConsoleEx.WriteLnThreaded("inside thread...");
                    //ConsoleEx.ReadLn("press ENTER");
                });

                t.Start();
                t.Join();
            }, "System.Threading.Thread");

            InstrumentedOperation.Test(() =>
            {
                const int SIZE = 5;
                CountdownEvent latch = new CountdownEvent(SIZE);

                for (int i = 0; i < SIZE; ++i)
                {
                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        var j = (int)state;
                        ConsoleEx.WriteLnThreaded("inside thread pool {0}...", j);

                        Thread.Sleep(RandomGenerator.GetRandomInt32(1000, 5000));
                        
                        latch.Signal();
                    }, i + 1);
                }

                latch.Wait();

            }, "System.Threading.ThreadPool");

            var a = new Action(() => {
                ConsoleEx.WriteLnThreaded("inside Parallel.Invoke...");
                Thread.Sleep(RandomGenerator.GetRandomInt32(1000, 5000));
            });

            InstrumentedOperation.Test(() =>
            {
                Parallel.Invoke
                (
                    a,
                    a,
                    a //, ...
                );
            }, "System.Threading.Tasks.Parallel.Invoke");

            InstrumentedOperation.Test(() =>
            {
                const int SIZE = 50;
                List<string> xs = new List<string>(SIZE);
                for (int i = 0; i < SIZE; ++i)
                    xs.Add(String.Format("inside Parallel.ForEach item #{0}...", i + 1));

                Parallel.ForEach
                (
                    xs,

                    x =>
                    {
                        ConsoleEx.WriteLnThreaded(x);
                        Thread.Sleep(RandomGenerator.GetRandomInt32(1000, 5000));
                    }
                );
            }, "System.Threading.Tasks.Parallel.ForEach");
                        
            InstrumentedOperation.Test(() =>
            {
                var a2 = new Action(() => {
                    ConsoleEx.WriteLnThreaded("inside Task...");
                    Thread.Sleep(RandomGenerator.GetRandomInt32(1000, 5000));
                });

                var t1 = Task.Factory.StartNew(a2);
                var t2 = Task.Factory.StartNew(a2);
                var t3 = Task.Factory.StartNew(a2);
                var t4 = Task.Factory.StartNew(a2);
                //...
                Task.WaitAll(t1, t2, t3, t4);
            }, "System.Threading.Tasks.Task");
        }
    }

    static class Program
    {
        public static void Run(Action action, string text = null)
        {
            ConsoleEx.SetTitle(text ?? String.Empty);
            action();
        }

        static void Main(string[] args)
        {
            ConsoleEx.WriteLnThreaded("Main thread");
            
            //Run(TestProgram.RunSpawningThreads, "RunSpawningThreads");

            //Run(TestProgram.RunRandomizedArray, "RandomizedArray");

            //Run(ThreadsProgram.Run, "Threads and Synchronization");

            //Run(StructuredParallelismProgram.Run, "StructuredParallelismProgram");

            //for (int i = 0; i < 3; ++i) Run(ClassicMailboxProgram.Run, "ClassicMailbox");
            //Run(ClassicMailboxProgram.RunTypeMsg, "ClassicMailbox typing message");

            //Run(SynchronizationPrimitivesProgram.Run, "SynchronizationPrimitives");

            //Run(LimitParallelismProgram.Run, "LimitParallelism");

            //Run(() => NamesProgram.Run(), "Names");

            //Run(ValueHolderProgram.Run, "ValueHolder");

            //Run(AsyncRunLoopProgram.Run, "AsyncRunLoop");

            //Run(ActiveObjectProgram.Run, "ActiveObject"); //.NET 4.5

            ConsoleEx.ReadLn("Waiting...");
        }
    }
}
