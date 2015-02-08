//Sample provided by Fabio Galuppo 
//February 2015 

using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpFPIntro
{
    static partial class FilterByProgram
    {
        static IEnumerable<int> filterByCongruentModuleN(IEnumerable<int> xs, int b, int n)
        {
            //http://en.wikipedia.org/wiki/Modular_arithmetic

            IEnumerator<int> exs = xs.GetEnumerator();
            List<int> ys = new List<int>();
            while (exs.MoveNext())
            {
                int a = exs.Current;
                if ((a - b) % n == 0)
                {
                    ys.Add(a);
                }
            }
            return ys;
        }

        static IEnumerable<int> filterBy(IEnumerable<int> xs, Func<int, bool> predicate)
        {
            IEnumerator<int> exs = xs.GetEnumerator();
            List<int> ys = new List<int>();
            while (exs.MoveNext())
            {
                int x = exs.Current;
                if (predicate(x))
                {
                    ys.Add(x);
                }
            }
            return ys;
        }

        static IEnumerable<T> filterBy<T>(IEnumerable<T> xs, Func<T, bool> predicate)
        {
            IEnumerator<T> exs = xs.GetEnumerator();
            List<T> ys = new List<T>();
            while (exs.MoveNext())
            {
                T x = exs.Current;
                if (predicate(x))
                {
                    ys.Add(x);
                }
            }
            return ys;
        }

        sealed class FilterEnumerable<T> : IEnumerable<T>, IEnumerator<T>
        {
            IEnumerable<T> source;
            Func<T, bool> predicate;
            T current;

            int state;
            IEnumerator<T> enumerator;

            public FilterEnumerable(IEnumerable<T> source, Func<T, bool> predicate)
            {
                Dispose(); //clean state

                this.source = source;
                this.predicate = predicate;
            }

            public IEnumerator<T> GetEnumerator()
            {
                state = 1;
                return this;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                state = 1;
                return this;
            }

            public T Current
            {
                get { return current; }
            }

            public void Dispose()
            {
                state = -1;
                enumerator = null;
                current = default(T);
            }

            object System.Collections.IEnumerator.Current
            {
                get { return current; }
            }

            public bool MoveNext()
            {
                switch (state)
                {
                    case 1:
                        enumerator = source.GetEnumerator();
                        state = 2;
                        goto case 2;
                    case 2:
                        while (enumerator.MoveNext())
                        {
                            var x = enumerator.Current;
                            if (predicate(x))
                            {
                                current = x;
                                return true;
                            }
                        }
                        Dispose();
                        break;
                }
                return false;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        internal static IEnumerable<T> lazyFilterBy<T>(IEnumerable<T> xs, Func<T, bool> predicate)
        {
            return new FilterEnumerable<T>(xs, predicate);
        }
    }

    static class LazyExtensions
    {
        public static IEnumerable<T> LazyFilterBy<T>(this IEnumerable<T> xs, Func<T, bool> predicate)
        {
            return FilterByProgram.lazyFilterBy(xs, predicate);
        }
    }

    static partial class FilterByProgram
    {
        public static void Test1()
        {
            IEnumerable<int> xs = Enumerable.Range(0, 13);
            foreach (var x in filterByCongruentModuleN(xs, 3, 2))
            {
                Console.WriteLine(x);
            }

            foreach (var x in filterBy(xs, x => (x & 0x1) == 0))
            {
                Console.WriteLine(x);
            }

            IEnumerable<string> ys = "Hello Functional Programming with C#".Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Func<string, bool> p = y => y.Length > 5;
            IEnumerable<string> ys0 = filterBy(ys, p);
            foreach (var y in ys0)
            {
                Console.WriteLine(y);
            }

            IEnumerable<string> ys1 = lazyFilterBy(ys, p);
            foreach (var y in ys1)
            {
                Console.WriteLine(y);
            }

            foreach (var y in ys.LazyFilterBy(p))
            {
                Console.WriteLine(y);
            }

            foreach (var y in ys.Where(p))
            {
                Console.WriteLine(y);
            }
        }
    }

}
