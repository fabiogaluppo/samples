//Sample provided by Fabio Galuppo
//August 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupportLibrary
{
    public static class ArrayExtensions
    {
        private static void Swap<T>(T[] a, int i, int j)
        {
            T temp = a[i];
            a[i] = a[j];
            a[j] = temp;
        }

        [ThreadStatic]
        private static Random Rnd_;

        public static T[] Shuffle<T>(this T[] a)
        {
            if (null == Rnd_) Rnd_ = new Random();
            for (int i = 0; i < a.Length; ++i)
                Swap(a, i, Rnd_.Next(i, a.Length));
            return a;
        }

        public static T[] ImmutableShuffle<T>(this T[] a)
        {
            if (typeof(T).IsValueType)
                return Shuffle(a.ToArray());

            T[] result = new T[a.Length];
            for(int i = 0; i < a.Length; ++i)
                result[i] = Cloning.BySerialization.DeepCopy(a[i]);
            return Shuffle(result);
        }
    }
}
