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
    public static class ValueHolderProgram
    {
        public static void Run()
        {
            var v = new ValueHolder<Dictionary<string, string>>();
            
            var wrappedDictionaryFactory = new Func<IEnumerable<Tuple<string, string>>, Func<Dictionary<string, string>>>(xs =>
            {
                return new Func<Dictionary<string, string>>(() =>
                {
                    //long run operation...
                    Thread.Sleep(1000 * RandomGenerator.GetRandomInt32(2, 5));
                    var d = new Dictionary<string, string>();
                    foreach(var x in xs) d.Add(x.Item1, x.Item2);
                    return d;
                });
            });

            var ys = new List<Tuple<string, string>>();
            for (int i = 0; i < 5; ++i) ys.Add(Tuple.Create("key #" + i, "value #" + i));
            
            InstrumentedOperation.Test(() => v.GetValue(wrappedDictionaryFactory(ys)), "creating value");
            InstrumentedOperation.Test(() => v.GetValue(wrappedDictionaryFactory(ys)), "getting value");
            v.Renew();
            InstrumentedOperation.Test(() => v.GetValue(wrappedDictionaryFactory(ys)), "creating value after renew");

            for (int i = 5; i < 6; ++i) ys.Add(Tuple.Create("key #" + i, "value #" + i));
            InstrumentedOperation.Test(() => v.GetValue(wrappedDictionaryFactory(ys), renew: true), "creating value after renew");

            var dictionaryFactory = wrappedDictionaryFactory(ys);
            InstrumentedOperation.Test(() =>
                Parallel.Invoke
                (
                    () => v.GetValue(dictionaryFactory),
                    () => v.GetValue(dictionaryFactory),
                    () => v.GetValue(dictionaryFactory)
                ), "getting values");

            InstrumentedOperation.Test(() =>
                Parallel.Invoke
                (
                    () => v.GetValue(dictionaryFactory),
                    () => v.GetValue(dictionaryFactory, renew: true),
                    () => v.GetValue(dictionaryFactory)
                ), "getting values after renew");
        }
    }

    public sealed class ValueHolder<T>
    {
        public enum State
        {
            Empty,
            RenewRequested,
            Constructing,
            Constructed
        };

        private const int CACHED_STATE_EMPTY = 0;
        private const int CACHED_STATE_CONSTRUCTING = 1;
        private const int CACHED_STATE_CONSTRUCTED = 2;

        private int RenewRequested_ = 0;
        private int CachedStateInt32_ = CACHED_STATE_EMPTY;
        private object Lock_ = new object();

        private T CachedValue_;
        
        public T GetValue(Func<T> valueFactory, bool renew = false) //thread-safe method
        {
            if (null == valueFactory)
                throw new ArgumentNullException("valueFactory");

            lock (Lock_)
            {
                #region "ValueHolder State Machine"
                if (renew || IsRenewRequested) //request for rebuild the cache
                    RenewAccepted();

                switch (CachedState)
                {
                    case State.Constructed: //return current cache
                        return CachedValue_;
                    case State.RenewRequested:
                        goto case State.Empty;
                    case State.Empty: //build the new cache
                        try
                        {
                            CachedState = State.Constructing;
                            CachedValue_ = valueFactory();
                            CachedState = State.Constructed;
                        }
                        catch (Exception)
                        {
                            CachedState = State.Empty;
                            throw;
                        }
                        goto case State.Constructed;
                    default: //invalid state                            
                        CachedState = State.Empty;
                        throw new InvalidOperationException("class invariant violated");
                }
                #endregion
            }
        }

        public State CachedState
        {
            get
            {
                if (IsRenewRequested)
                    return State.RenewRequested;

                switch (CachedStateInt32)
                {
                    case CACHED_STATE_EMPTY:
                        return State.Empty;
                    case CACHED_STATE_CONSTRUCTING:
                        return State.Constructing;
                    case CACHED_STATE_CONSTRUCTED:
                        return State.Constructed;
                    default:
                        throw new InvalidOperationException();
                }
            }

            private set
            {
                switch (value)
                {
                    case State.Empty:
                        CachedStateInt32 = CACHED_STATE_EMPTY;
                        break;
                    case State.Constructing:
                        CachedStateInt32 = CACHED_STATE_CONSTRUCTING;
                        break;
                    case State.Constructed:
                        CachedStateInt32 = CACHED_STATE_CONSTRUCTED;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public void Renew()
        {
            Thread.VolatileWrite(ref RenewRequested_, 1);
        }

        #region "private methods"
        private bool IsRenewRequested
        {
            get
            {
                return 1 == Thread.VolatileRead(ref RenewRequested_);
            }
        }

        private int CachedStateInt32
        {
            set { Thread.VolatileWrite(ref CachedStateInt32_, value); }
            get { return Thread.VolatileRead(ref CachedStateInt32_); }
        }

        private void RenewAccepted()
        {
            Thread.VolatileWrite(ref RenewRequested_, 0);
            CachedState = State.Empty;
        }
        #endregion
    }
}
