//Sample provided by Fabio Galuppo 
//February 2015 

using System;

namespace CSharpFPIntro
{
    static class EagerOrLazyProgram
    {
        static int EagerFunc(int i, int j)
        {
            //prepare and use
            Console.WriteLine("eager");
            return i + j;
        }

        static Func<int> LazyFunc(int i, int j)
        {
            //prepare
            return () =>
            {   
                //(prepare and) use
                Console.WriteLine("lazy");
                return i + j;
            };
        }

        public static void Test1()
        {
            int r0 = EagerFunc(1, 2);
            int r1 = LazyFunc(1, 2)();
        }   
    }
}
