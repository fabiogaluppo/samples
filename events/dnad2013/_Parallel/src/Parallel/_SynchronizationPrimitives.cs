//Sample provided by Fabio Galuppo
//August 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SupportLibrary;

using System.Threading;

namespace _Parallel
{
    public static class SynchronizationPrimitivesProgram
    {
        static void DoNothing()
        {
        }

        public static void Run()
        {
            const int SIZE = 10 * 1000000;
            //const int SIZE = 1000000;
            //const int SIZE = 100000;
            //const int SIZE = 10000;
            //const int SIZE = 1000;

            var result = InstrumentedOperation.Test(() =>
            {
                for (int i = 0; i < SIZE; ++i)
                {
                    DoNothing();
                }
            }, "Empty method [user]", SIZE);

            object Lock = new object();
            result = InstrumentedOperation.Test(() =>
            {
                for (int i = 0; i < SIZE; ++i)
                {
                    lock (Lock)
                        DoNothing();
                }
            }, "lock [user]", SIZE);

            Mutex m = new Mutex();
            result = InstrumentedOperation.Test(() =>
            {
                for (int i = 0; i < SIZE; ++i)
                {
                    m.WaitOne();
                        DoNothing();
                    m.ReleaseMutex();
                }
            }, "Mutex [kernel]", SIZE);
            m.Close();

            Semaphore s = new Semaphore(1, 1);
            result = InstrumentedOperation.Test(() =>
            {
                for (int i = 0; i < SIZE; ++i)
                {
                    s.WaitOne();
                    DoNothing();
                    s.Release();
                }
            }, "Semaphore [kernel]", SIZE);
            s.Close();

            SemaphoreSlim ss = new SemaphoreSlim(1, 1);
            result = InstrumentedOperation.Test(() =>
            {
                for (int i = 0; i < SIZE; ++i)
                {
                    ss.Wait();
                    DoNothing();
                    ss.Release();
                }
            }, "SemaphoreSlim [user]", SIZE);
            ss.Dispose();

            ManualResetEventSlim mre = new ManualResetEventSlim(true);
            result = InstrumentedOperation.Test(() =>
            {
                for (int i = 0; i < SIZE; ++i)
                {
                    mre.Wait();
                    DoNothing();
                    mre.Set();
                }
            }, "ManualResetEventSlim [user]", SIZE);
            mre.Dispose();

            CountdownEvent c = new CountdownEvent(0);
            result = InstrumentedOperation.Test(() =>
            {
                for (int i = 0; i < SIZE; ++i)
                {
                    c.Wait();
                    DoNothing();
                    c.Reset(0);
                }
            }, "CountdownEvent [user]", SIZE);
            c.Dispose();
        }
    }
}
