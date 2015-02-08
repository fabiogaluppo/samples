//Sample provided by Fabio Galuppo 
//February 2015 

using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpFPIntro
{
    static class FoldProgram
    {
        //short-circuit
        static bool And(this IEnumerable<bool> xs)
        {
            foreach (var x in xs)
                if (!x) return false;
            return true;
        }

        public static void Test1()
        {
            IEnumerable<int> xs = Enumerable.Range(1, 10);

            //fold
            int a = xs.Aggregate((acc, x) => acc + x);
            
            //foldBack
            int b = xs.Reverse().Aggregate((acc, x) => acc + x);

            IEnumerable<bool> ys = new bool[] { true, false, true, false };

            bool c = ys.Aggregate((acc, y) => acc && y); //non-short-circuit
            bool d = ys.And();

            Console.WriteLine(a);
            Console.WriteLine(b);
            Console.WriteLine(c);
            Console.WriteLine(d);
        }
    }
}
