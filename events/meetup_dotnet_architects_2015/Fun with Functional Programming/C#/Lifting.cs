//Sample provided by Fabio Galuppo 
//February 2015 

using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpFPIntro
{
    static class LiftingProgram
    {
        static float IntToFloat(int x)
        {
            return Convert.ToSingle(x);
        }

        static Func<Nullable<TInput>, Nullable<TOutput>> Lift<TInput, TOutput>(Func<TInput, TOutput> f)
            where TInput : struct
            where TOutput : struct
        {
            return x =>
            {
                if (x.HasValue)
                    return new Nullable<TOutput>(f(x.Value));
                return new Nullable<TOutput>();
            };
        }

        public static void Test1()
        {
            var xs = Enumerable.Range(1, 10);

            IEnumerable<float> ys = xs.Select(x => 10.0f * x).ToArray();

            IEnumerable<float> zs = xs.Select(IntToFloat).ToArray();

            IEnumerable<Nullable<int>> ws = Enumerable.Range(1, 10).Select(x => new Nullable<int>(x));

            //IEnumerable<Nullable<float>> vs = ws.Select(IntToFloat).ToArray(); //?
            IEnumerable<Nullable<float>> vs = ws.Select(Lift<int, float>(IntToFloat)).ToArray();
            foreach (var v in vs)
                if (v.HasValue) Console.Write("{0:0.0} ", v);
                else Console.Write("Null ");
            Console.WriteLine();
        }
    }
}
