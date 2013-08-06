//Sample provided by Fabio Galuppo
//August 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SupportLibrary
{
    public interface IRandomGeneration
    {
        int[] GetRandomizedArray(int size, int minInclusive, int maxInclusive);
    }

    public sealed class RandomGenerator
    {
        [ThreadStatic]
        private static Random Rnd_;
        
        public static int GetRandomInt32(int minInclusive = 0, int maxInclusive = Int32.MaxValue - 1)
        {
            if (null == Rnd_) Rnd_ = new Random();
            return Rnd_.Next(minInclusive, maxInclusive + 1);
        }
        
        private class _Parallel : IRandomGeneration
        {
            public int[] GetRandomizedArray(int size, int minInclusive, int maxInclusive)
            {
                var arr = new int[size];
                Parallel.For(0, size,
                    () => new Random(),

                    (i, state, rnd) =>
                    {
                        arr[i] = rnd.Next(minInclusive, maxInclusive + 1);
                        return rnd;
                    },

                    rnd => { }
                );

                return arr;
            }
        }

        private class _Sequential : IRandomGeneration
        {
            public int[] GetRandomizedArray(int size, int minInclusive, int maxInclusive)
            {
                var rnd = new Random();
                var arr = new int[size];
                for (int i = 0; i < size; ++i)
                    arr[i] = rnd.Next(minInclusive, maxInclusive + 1);
                return arr;
            }
        }

        static IRandomGeneration P_, S_;

        public static IRandomGeneration AsParallel()
        {
            return GetSingleton(ref P_, () => new _Parallel()); ;
        }

        public static IRandomGeneration AsSequential()
        {
            return GetSingleton(ref S_, () => new _Sequential());
        }

        private static IRandomGeneration GetSingleton<T>(ref T target, Func<T> valueFactory) 
            where T : class, IRandomGeneration
        {
            if (null == target)
                LazyInitializer.EnsureInitialized(ref target, valueFactory);
            return target;
        }
    }
}
