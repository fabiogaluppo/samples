//Sample provided by Fabio Galuppo
//August 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

using SupportLibrary;

namespace _Parallel
{
    public static class LimitParallelismProgram
    {
        public static void Run()
        {
            var limit1 = LimitParallelism.ForShortRunning(5);
            var ts = new List<Thread>();
            for (int i = 0; i < 3; ++i)
                ts.Add(new Thread(() => _Threads.RunLimit(50, 101, limit1, "short running")));
            foreach (var t in ts) t.Start();

            var limit2 = LimitParallelism.ForLongRunning(5);
            var ts2 = new List<Thread>();
            for (int i = 0; i < 3; ++i)
                ts2.Add(new Thread(() => _Threads.RunLimit(2000, 4001, limit2, "long running")));
            foreach (var t in ts2) t.Start();

            ConsoleEx.ReadLn("\nPress ENTER to dispose...\n");

            ThreadPool.QueueUserWorkItem(state => limit1.Dispose());
            ThreadPool.QueueUserWorkItem(state => limit1.Dispose());
            ThreadPool.QueueUserWorkItem(state => limit1.Dispose());

            limit1.Dispose();
            limit2.Dispose();
        }
    }

    public abstract class LimitParallelism : IDisposable
    {
        protected internal LimitParallelism(int maxCount)
        {
            if (maxCount < 1)
                throw new ArgumentException("must be at least equals 1", "maxCount");
            MaxCount = maxCount;
        }

        ~LimitParallelism()
        {
            DisposeCore();
        }

        public void Enter() { EnterCore(); }
        public void Leave() { LeaveCore(); }        
        public int Count { get { return CountCore; } }
        public int MaxCount { get; private set; }

        protected abstract void EnterCore();
        protected abstract void LeaveCore();
        protected abstract int CountCore{ get; }

        int Disposed_ = 0;
        public void Dispose()
        { 
            var disposed = Interlocked.CompareExchange(ref Disposed_, 1, 0);
            if (0 == disposed)
            {
                GC.SuppressFinalize(this);
                DisposeCore();
            }
        }

        protected abstract void DisposeCore();

        public static LimitParallelism ForLongRunning(int maxCount)
        {
            return new LimitParallelismForLongRunning(maxCount);
        }

        public static LimitParallelism ForShortRunning(int maxCount)
        {
            return new LimitParallelismForShortRunning(maxCount);
        }
    }

    internal sealed class LimitParallelismForLongRunning : LimitParallelism
    {
        private Semaphore S_;
        private int Count_;

        internal LimitParallelismForLongRunning(int maxCount)
            : base(maxCount)
        {
            S_ = new Semaphore(maxCount, maxCount);
            Count_ = maxCount; 
        }

        protected override void LeaveCore()
        {
            if (null != S_)
            {
                S_.Release(1);
                Interlocked.Increment(ref Count_);
            }
        }

        protected override void EnterCore()
        {
            if (null != S_)
            {
                S_.WaitOne();
                Interlocked.Decrement(ref Count_);
            }
        }

        protected override int CountCore 
        {
            get
            {
                var count = Thread.VolatileRead(ref Count_);
                return null != S_ ? (count > 0 ? count : 0) : 0;
            }
        }

        protected override void DisposeCore()
        {
            var s = Interlocked.CompareExchange(ref S_, null, S_);
            if (null != s) s.Close();
        }
    }

    internal sealed class LimitParallelismForShortRunning : LimitParallelism
    {
        private SemaphoreSlim S_;

        internal LimitParallelismForShortRunning(int maxCount)
            : base(maxCount)
        {
            S_ = new SemaphoreSlim(maxCount, maxCount);
        }

        protected override void LeaveCore()
        {
            if (null != S_) S_.Release(1);
        }

        protected override void EnterCore()
        {
            if (null != S_) S_.Wait();
        }

        protected override int CountCore
        {
            get
            {
                return null != S_ ? S_.CurrentCount : 0;
            }
        }

        protected override void DisposeCore()
        {
            var s = Interlocked.CompareExchange(ref S_, null, S_);
            if (null != s) s.Dispose();
        }
    }
}
