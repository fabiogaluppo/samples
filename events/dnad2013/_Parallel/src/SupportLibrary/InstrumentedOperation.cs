//Sample provided by Fabio Galuppo
//August 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupportLibrary
{
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
            if (dbgWriteLn) Tracer.TraceLn(temp.ToString());
            return temp;
        }

        public static InstrumentedOperation Test(Action action, int normalizer = 1, bool dbgWriteLn = true)
        {
            return Test(action, String.Empty, normalizer, dbgWriteLn);
        }

        public static InstrumentedOperation<T> Test<T>(Func<T> func, string text, int normalizer = 1, bool dbgWriteLn = true)
        {
            var temp = new InstrumentedOperation<T>(func, text, normalizer);
            if (dbgWriteLn) Tracer.TraceLn(temp.ToString());
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
}
