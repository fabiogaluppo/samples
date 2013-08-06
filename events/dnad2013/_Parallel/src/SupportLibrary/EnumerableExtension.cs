//Sample provided by Fabio Galuppo
//August 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportLibrary
{
    public static class EnumerableExtension
    {
        public static string MakeString<T>(this IEnumerable<T> xs)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var x in xs) sb.Append(x);
            return sb.ToString();
        }

        public static string MakeString<T>(this IEnumerable<T> xs, string sep)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var x in xs.Init()) sb.AppendFormat("{0}{1}", x, sep);
            sb.Append(xs.Last());
            return sb.ToString();
        }

        public static string MakeString<T>(this IEnumerable<T> xs, string start, string sep, string end)
        {
            return String.Format("{0}{1}{2}", start, MakeString(xs, sep), end);
        }

        public static T Head<T>(this IEnumerable<T> xs)
        {
            return xs.First();
        }

        public static IEnumerable<T> Tail<T>(this IEnumerable<T> xs)
        {
            return xs.Skip(1);
        }

        public static IEnumerable<T> Init<T>(this IEnumerable<T> xs)
        {
            return xs.Take(Math.Max(0, xs.Count() - 1));
        }
    }
}
