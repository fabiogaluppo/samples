//Sample provided by Fabio Galuppo 
//February 2015 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpFPIntro
{
    public static class EnumerableExtension
    {
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
            IEnumerator<T> e = xs.GetEnumerator();
            bool cont = e.MoveNext();
            while (cont)
            {
                var x = e.Current;
                cont = e.MoveNext();
                if (cont)
                    yield return x;
            }
        }

        #region "MakeString"
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
        #endregion
    }


    static class ListFuncsProgram
    {
        public static void Test1()
        {
            IEnumerable<int> xs = Enumerable.Range(1, 10);

            Console.WriteLine("xs: " + xs.MakeString("{", ", ", "}"));
            Console.WriteLine("Head of xs: " + xs.Head());
            Console.WriteLine("Tail of xs: " + xs.Tail().MakeString("{", ", ", "}"));
            Console.WriteLine("Last of xs: " + xs.Last());
            Console.WriteLine("Init of xs: " + xs.Init().MakeString("{", ", ", "}"));
        }
    }
}
