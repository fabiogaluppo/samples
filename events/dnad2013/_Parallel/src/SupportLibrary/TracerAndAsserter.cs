//Sample provided by Fabio Galuppo
//August 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SupportLibrary
{
    public static class Tracer
    {
        static Tracer()
        {
            Trace.AutoFlush = true;
            SetupTrace(new TextWriterTraceListener(Console.Out));
        }

        public static void TraceLn(string format, params object[] args)
        {
            Trace.WriteLine(String.Format(format, args));
        }

        public static void SetupTrace(params TraceListener[] listeners)
        {
            Trace.Listeners.AddRange(listeners);
        }
    }

    public static class Asserter
    {
        public static class WithException
        {
            public static void Assert(bool condition, string format = null, params object[] args)
            {
                if (null == format)
                    Trace.Assert(condition);
                else
                    Trace.Assert(condition, String.Format(format, args));
            }

            public static void Assert(bool condition, string text)
            {
                if (!condition)
                    throw new InvalidOperationException(text);
            }
        }

        public static void Assert(bool condition, string format = null, params object[] args)
        {
            if(!condition) 
                Tracer.TraceLn("Assertion doesn't hold" + 
                    (!String.IsNullOrEmpty(format) ? ": " + format : String.Empty), args);
        }
    }
}
