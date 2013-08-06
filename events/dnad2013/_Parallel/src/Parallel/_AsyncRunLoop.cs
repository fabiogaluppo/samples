//Sample provided by Fabio Galuppo
//August 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using SupportLibrary;

namespace _Parallel
{
    public static class AsyncRunLoopProgram
    {
        public static void Run()
        {
            var runLoop = new AsyncRunLoop();
            
            runLoop.AddObserver<string>(msg =>
            {
                ConsoleEx.WriteLnThreaded(msg);
            });

            runLoop.AddObserver<int[]>(msg =>
            {
                ConsoleEx.WriteLn(msg);
            });

            runLoop.MessageIgnored += new Action<object>(runLoop_MessageIgnored);

            runLoop.Post("Hello");
            runLoop.Post(RandomGenerator.AsSequential().GetRandomizedArray(10, 0, 1));
            runLoop.Post("World");
            runLoop.Post(Guid.NewGuid().ToByteArray().Select(b => (int)b).ToArray());
            runLoop.Post(".NET");
            runLoop.Post(RandomGenerator.AsSequential().GetRandomizedArray(5, 1, 5));
            runLoop.Post(Guid.NewGuid().GetType());

            string line;
            while (String.Empty != (line = ConsoleEx.ReadLn())) runLoop.Post(line);

            runLoop.PostEnd();
        }

        static void runLoop_MessageIgnored(object obj)
        {
            ConsoleEx.WriteLnThreaded("Message ignored " + obj.ToString());
        }
    }
    
    public sealed class AsyncRunLoop : IDisposable
    {
        private static readonly object END_MESSAGE = new object();

        private System.Collections.Concurrent.ConcurrentDictionary<Type, Action<object>> D_ = 
            new System.Collections.Concurrent.ConcurrentDictionary<Type, Action<object>>();

        private System.Collections.Concurrent.ConcurrentQueue<object> Q_ = 
            new System.Collections.Concurrent.ConcurrentQueue<object>();

        ManualResetEventSlim Waiter_ = new ManualResetEventSlim(false);

        public event Action<object, Exception> ExceptionRaised;
        public event Action<object> MessageIgnored;
        public event Action EndMessageArrived;

        Thread RunLoopThread_;

        public AsyncRunLoop()
        {
            RunLoopThread_ = new Thread(Receiver) { IsBackground = false };
            RunLoopThread_.Start();
        }

        ~AsyncRunLoop()
        {
            FreeResources();
        }

        public void AddObserver<T>(Action<T> handler) where T : class
        {
            if (!D_.TryAdd(typeof(T), arg => handler((T)arg)))
                throw new InvalidOperationException();
        }

        public void RemoveObserver<T>(Action<T> handler) where T : class
        {
            Action<object> temp;
            if (!D_.TryRemove(typeof(T), out temp))
                throw new InvalidOperationException();
        }

        private void FreeResources()
        {
            PostEnd();
        }

        private void Receiver()
        {
            bool forever = true;
            while (forever)
            {
                object context;
                if (Q_.TryDequeue(out context))
                {
                    if (END_MESSAGE == context)
                    {
                        if (null != EndMessageArrived)
                            EndMessageArrived();

                        forever = false;

                        Q_ = null;
                        D_ = null;
                        Waiter_.Dispose();
                        Waiter_ = null;
                        RunLoopThread_ = null;
                        ExceptionRaised = null;
                        MessageIgnored = null;
                        EndMessageArrived = null;

                        return;
                    }

                    Action<object> handler;
                    if (D_.TryGetValue(context.GetType(), out handler))
                    {
                        try
                        {
                            handler(context);
                        }
                        catch (Exception e)
                        {
                            if (null != ExceptionRaised)
                                ExceptionRaised(context, e);
                        }
                    }
                    else
                    {
                        if (null != MessageIgnored)
                            MessageIgnored(context);
                    }
                }
                else
                {
                    Waiter_.Reset();
                    Waiter_.Wait(5000);
                }
            }
        }

        public void Post<T>(T message)
        {
            if (null != Q_)
            {
                Q_.Enqueue(message);
                var count = Q_.Count;
                Waiter_.Set();
            }
        }

        public void PostEnd()
        {
            Post(END_MESSAGE);
        }

        public void Abort()
        {
            if (null != RunLoopThread_)
            {
                GC.SuppressFinalize(this);
                RunLoopThread_.Abort();
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            FreeResources();
        }
    }
}