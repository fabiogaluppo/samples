//Sample provided by Fabio Galuppo
//August 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

using System.Collections.Concurrent;

using SupportLibrary;

namespace _Parallel
{
    public static class ClassicMailboxProgram
    {
        //const int SIZE = 10 * 1000000;
        const int SIZE = 1000000;
        //const int SIZE = 100000;
        //const int SIZE = 10000;
        //const int SIZE = 1000;
        //const int SIZE = 10;

        public static void RunTypeMsg()
        {
            var m = ClassicMailbox.AsLockFree((string msg) =>
            {
                //Console.WriteLine(String.Format("\n\t[dst:{0}]{1}", Thread.CurrentThread.ManagedThreadId, msg));
            });

            string line;
            while((line = ConsoleEx.ReadLn("Type msg or ENTER to exit: ")) != String.Empty)
            {
                m.Send(String.Format("[src:{0}] = {1}", Thread.CurrentThread.ManagedThreadId, line));
            }
            m.CancelAndWait();
            Console.WriteLine("[src:{0}] = Mailbox stopped...", Thread.CurrentThread.ManagedThreadId);
        }

        public static void Run()
        {
            var m1 = ClassicMailbox.AsLock((string msg) => 
            { 
                //Console.WriteLine(String.Format("[dst:{0}]{1}", Thread.CurrentThread.ManagedThreadId, msg)); 
            });

            var result = InstrumentedOperation.Test(() =>
            {
                for (int i = 0; i < SIZE; ++i)
                {
                    m1.Send(String.Format("[src:{0}] Message #{1}", Thread.CurrentThread.ManagedThreadId, i+1));
                }

                m1.CancelAndWait();

            }, "Queue<T> and lock with signaling", SIZE);


            var m2 = ClassicMailbox.AsLockFree((string msg) =>
            {
                //Console.WriteLine(String.Format("[dst:{0}]{1}", Thread.CurrentThread.ManagedThreadId, msg));
            });

            result = InstrumentedOperation.Test(() =>
            {
                for (int i = 0; i < SIZE; ++i)
                {
                    m2.Send(String.Format("[src:{0}] Message #{1}", Thread.CurrentThread.ManagedThreadId, i + 1));
                }

                m2.CancelAndWait();

            }, "BlockingCollection<T> and lock free queue", SIZE);
        }
    }

    public static class ClassicMailbox
    {
        public static ClassicMailbox<T> AsLock<T>(Action<T> receiver)
        {
            return new ClassicMailboxUsingQueue<T>(receiver);
        }

        public static ClassicMailbox<T> AsLockFree<T>(Action<T> receiver)
        {
            return new ClassicMailboxUsingConcurrent<T>(receiver);
        }
    }
    
    public abstract class ClassicMailbox<T>
    {
        Action<T> Receiver_;
        static int Counter_ = 1;
        CountdownEvent Event_ = new CountdownEvent(1);
        int IsRunning_ = 0;

        internal ClassicMailbox(Action<T> receiver)
        {
            if (null == receiver)
                throw new ArgumentNullException("receiver");

            Receiver_ = receiver;

            var t = new Thread(new ThreadStart(Receive))
            {
                Name = String.Format("ClassicMailbox #{0}::{1}", Interlocked.Increment(ref Counter_), typeof(T).FullName),
                IsBackground = false
            };

            t.Start();

            Thread.VolatileWrite(ref IsRunning_, 1);
        }

        protected abstract T ReceiveCore(out bool cancel);

        private void Receive()
        {
            while (true)
            {
                bool cancel;
                var value = ReceiveCore(out cancel);

                if (cancel)
                {
                    Thread.VolatileWrite(ref IsRunning_, 0);
                    Event_.Signal();
                    return;
                }

                Receiver_(value);
            }
        }

        public void Wait(int millisecondsTimeout = Timeout.Infinite) { Event_.Wait(millisecondsTimeout); }

        public WaitHandle WaitHandle { get { return Event_.WaitHandle; } }

        protected abstract void SendCore(T message, bool cancel);

        public void Send(T message) 
        { 
            if (IsRunning) 
                SendCore(message, false); 
        }
        
        public void Cancel() 
        { 
            if (IsRunning) 
                SendCore(default(T), true); 
        }

        public void CancelAndWait() 
        {
            Cancel();
            Wait();
        }

        public bool IsRunning { get { return 1 == Thread.VolatileRead(ref IsRunning_); } }
    }

    internal sealed class ClassicMailboxUsingQueue<T> : ClassicMailbox<T>
    {
        Queue<Tuple<T, bool>> Q_ = new Queue<Tuple<T, bool>>();

        public ClassicMailboxUsingQueue(Action<T> receiver) 
            : base(receiver)
        {
        }

        protected override T ReceiveCore(out bool cancel)
        {
            Tuple<T, bool> message;
            lock (Q_)
            {
                while (Q_.Count == 0)
                    Monitor.Wait(Q_);
                message = Q_.Dequeue();
            }

            cancel = message.Item2;
            return message.Item1;
        }

        protected override void SendCore(T message, bool cancel)
        {
            lock (Q_)
            {
                Q_.Enqueue(Tuple.Create(message, cancel));
                Monitor.Pulse(Q_);
            }
        }
    }

    internal sealed class ClassicMailboxUsingConcurrent<T> : ClassicMailbox<T>
    {
        BlockingCollection<Tuple<T, bool>> BC_ = new BlockingCollection<Tuple<T, bool>>(new ConcurrentQueue<Tuple<T, bool>>());

        public ClassicMailboxUsingConcurrent(Action<T> receiver)
            : base(receiver)
        {
        }

        protected override T ReceiveCore(out bool cancel)
        {
            Tuple<T, bool> message = BC_.Take();
            cancel = message.Item2;
            return message.Item1;
        }

        protected override void SendCore(T message, bool cancel)
        {
            BC_.Add(Tuple.Create(message, cancel));
        }
    }
}
