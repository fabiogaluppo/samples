//Sample provided by Fabio Galuppo
//August 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupportLibrary
{
    public static class ConsoleEx
    {
        public static string ReadLn(string text = null, int delayMilliseconds = -1)
        {
            if (delayMilliseconds > 99) System.Threading.Thread.Sleep(delayMilliseconds);
            if (!String.IsNullOrEmpty(text)) Console.Write(text);
            return Console.ReadLine();
        }

        public static void WriteLnThreaded(string format, params object[] args)
        {
            var reformat = String.Format("[TID: {2,6}][MTID: {0,4}]: {1}", System.Threading.Thread.CurrentThread.ManagedThreadId, format, WindowsApi.GetCurrentThreadId());
            Console.WriteLine(reformat, args);
        }

        public static void WriteLn<T>(IEnumerable<T> xs, int count = 10, string text = null)
        {
            var len = Math.Min(xs.Count(), count);
            Console.WriteLine((text ?? String.Empty) + xs.Take(count).MakeString("[", "; ", "]"));
        }

        public static void SetTitle(string title)
        {
            var p = System.Diagnostics.Process.GetCurrentProcess();
            Console.Title = String.Format("[{0}:{1}] {2}", p.ProcessName, p.Id, title);
        }
    }
}
