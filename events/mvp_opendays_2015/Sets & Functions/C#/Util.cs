//Sample provided by Fabio Galuppo  
//March 2015  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;

namespace CSharpPlayground
{
    public sealed class Int32Comparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            if (x < y) return -1;
            if (x > y) return 1;
            return 0;
        }
    }

    public static partial class KeyValuePairExtensions
    {
        public static KeyValuePair<TValue, TKey> Transpose<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp)
        {
            return new KeyValuePair<TValue, TKey>(kvp.Value, kvp.Key);
        }
    }

    public static partial class EnumerableExtensions
    {
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> xs)
        {
            return xs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }

    public static partial class EnumerableExtensions
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
            int count = xs.Count();
            var xs_init = xs.Take(count - 1);
            foreach (var x in xs_init) sb.AppendFormat("{0}{1}", x, sep);
            if (count > 0) sb.Append(xs.Last());
            return sb.ToString();
        }

        public static string MakeString<T>(this IEnumerable<T> xs, string start, string sep, string end)
        {
            return start + MakeString(xs, sep) + end;
        }
    }

    public static partial class SerializationUtil
    {
        private static DataContractJsonSerializer GetSerializer<T>()
        {
            return new DataContractJsonSerializer(typeof(T));
        }

        public static string SerializeToJson<T>(T value)
        {
            var serializer = GetSerializer<T>();
            var serializationStream = new MemoryStream();
            serializer.WriteObject(serializationStream, value);
            serializationStream.Position = 0;
            using (var sr = new StreamReader(serializationStream))
                return sr.ReadToEnd();
        }

        public static T DeserializeFromJson<T>(string jsonString)
        {
            var serializer = GetSerializer<T>();
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
                return (T)serializer.ReadObject(ms);
        }
    }

    partial class Program
    {
        private static void PrintLn<T>(IEnumerable<T> xs)
        {
            Console.WriteLine(xs.MakeString("{", ", ", "}"));
        }

        private static void PrintLn<TKey, TValue>(IDictionary<TKey, TValue> dic)
        {
            PrintLn(dic.Select(kvp => kvp.Key + " -> " + kvp.Value));
        }

        private static void Run(Action f, string title)
        {
            Console.WriteLine(title + ":");
            f();
            Console.WriteLine(new string('-', 15));
        }
    }
}
