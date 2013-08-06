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
    public static class ActiveObjectProgram
    {
        public static void Run()
        {
            var x = new MyObject();
            x.Method1();
            x.Method2();

            var xsync = MyObject.NewActiveObjectSync();
            var xasync = MyObject.NewActiveObjectAsync();

            xsync.Method1().ContinueWith(r => ConsoleEx.WriteLnThreaded("Sync = {0}", r.Result));
            xasync.Method1().ContinueWith(r => ConsoleEx.WriteLnThreaded("Async = {0}", r.Result));

            xsync.Method2().ContinueWith(r => ConsoleEx.WriteLnThreaded("Sync = {0}", r.Result));
            xasync.Method2().ContinueWith(r => ConsoleEx.WriteLnThreaded("Async = {0}", r.Result));
        }
    }
    
    public interface IMyObject
    {
        double Method1();
        double Method2();
    }

    public interface IActiveObject
    {
        Task<double> Method1();
        Task<double> Method2();
    }

    public sealed class MyObject : IMyObject
    {
        public MyObject()
        {
            Id = "MyObject";
        }

        [ThreadStatic]
        private static string Id;

        public static IActiveObject NewActiveObjectSync()
        {
            return new Sync(new MyObject());
        }

        public static IActiveObject NewActiveObjectAsync()
        {
            return new Async(new MyObject());
        }

        private sealed class Sync : IActiveObject
        {
            private MyObject This_;

            public Sync(MyObject self)
            {
                This_ = self;
            }
            
            #region IActiveObject Members

            public Task<double> Method1()
            {
                Id = "Sync";
                return Task.FromResult(This_.Method1());
            }

            public Task<double> Method2()
            {
                Id = "Sync";
                return Task.FromResult(This_.Method2());
            }

            #endregion
        }

        private sealed class Async : IActiveObject
        {
            private MyObject This_;

            public Async(MyObject self)
            {
                This_ = self;
            }

            #region IActiveObject Members

            public Task<double> Method1()
            {
                return Task.Run(() => 
                {
                    Id = "Async";
                    return This_.Method1();
                });
            }

            public Task<double> Method2()
            {
                return Task.Run(() =>
                {
                    Id = "Async";
                    return This_.Method2();
                });
            }

            #endregion
        }

        public double Method1()
        {
            ConsoleEx.WriteLnThreaded("{0}.Method1", Id);
            return Math.E;
        }

        public double Method2()
        {
            Thread.Sleep(RandomGenerator.GetRandomInt32(3,6));
            ConsoleEx.WriteLnThreaded("{0}.Method2", Id);
            return Math.PI;
        }
    }
}