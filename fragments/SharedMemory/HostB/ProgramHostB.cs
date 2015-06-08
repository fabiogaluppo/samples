//Sample provided by Fabio Galuppo
//June 2015

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib;

namespace HostB
{
    static class EnumerableExtensions
    {
        public static string MakeString<T>(this IEnumerable<T> xs)
        {
            StringBuilder sb = new StringBuilder(xs.Count());
            foreach (var x in xs) sb.Append(x);
            return sb.ToString();
        }
    }

    class ProgramHostB
    {
        static void Main(string[] args)
        {
            Console.Title = "HostB";
            Console.ReadLine();
            using (var sm = SharedMemory.Open("SM1"))
            {
                using (var r = sm.AsReader())
                {
                    Console.WriteLine(r.ReadInt32());
                    Console.WriteLine(r.ReadInt32());
                    Console.WriteLine(r.ReadInt32());
                    Console.WriteLine(r.ReadBytes(5).MakeString());
                }
            }
            Console.ReadLine();
            using (var sm2 = SharedMemory.Open("SM2"))
            {
                using (var r = sm2.AsReader(0L, 1L * 1024))
                {
                    Console.WriteLine(r.ReadBytes((int)r.Size).MakeString());
                }
                Console.ReadLine();
                using (var r = sm2.AsReader(1L * 1024, 1L * 1024))
                {
                    Console.WriteLine(r.ReadBytes((int)r.Size).MakeString());
                }
                Console.ReadLine();
                using (var r = sm2.AsReader(0L, 1L * 1024))
                {
                    Console.WriteLine(r.ReadBytes((int)r.Size).MakeString());
                }
                Console.ReadLine();
            }
        }
    }
}
