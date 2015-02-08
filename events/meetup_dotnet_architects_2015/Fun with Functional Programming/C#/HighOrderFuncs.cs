//Sample provided by Fabio Galuppo 
//February 2015 

using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpFPIntro
{
    //F# version:
    /*
     let multiplyBy x y = x * y
     let duplicate = multiplyBy 2
     let quadruplicate = multiplyBy 4

     let a = duplicate 10 
     let b = quadruplicate 10
     let c (f: unit -> int) = duplicate (f())
     let d (f: unit -> int) = fun () -> quadruplicate (f()) 
    */

    static class HighOrderFuncsProgram
    {
        static Func<int, int> multiplyBy(int x)
        {
            return y => x * y;
        }

        static Func<int, int> duplicate()
        {
            return multiplyBy(2);
        }

        static Func<int, int> quadruplicate()
        {
            return multiplyBy(4);
        }

        static void Iterate<T>(IEnumerable<T> xs, Action<T> f)
        {
            foreach (var x in xs) f(x);            
        }

        public static void Test1()
        {
            int a = duplicate()(10);
            int b = quadruplicate()(10);
            var c = new Func<Func<int>, int>(f => duplicate()(f()));
            var d = new Func<Func<int>, Func<int>>(f => new Func<int>(() => quadruplicate()(f())));

            Console.WriteLine(a);
            Console.WriteLine(b);
            Console.WriteLine(c(() => 10));
            Console.WriteLine(d(() => 10));
            Console.WriteLine(d(() => 10)());

            Iterate(Enumerable.Range(1, 20), x => Console.Write(x + " "));
            Console.WriteLine();
        }
    }
}
