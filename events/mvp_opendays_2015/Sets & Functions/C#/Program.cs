//Sample provided by Fabio Galuppo  
//March 2015  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPlayground
{
    partial class Program
    {
        #region "Sets and Set operations"
        private static bool Equals<T>(IEnumerable<T> xs, IEnumerable<T> ys, IComparer<T> comparer)
        {
            if (xs.Count() != ys.Count())
                return false;
            foreach (var x_y in xs.Zip(ys, (x, y) => Tuple.Create(x, y)))
            {
                var x = x_y.Item1;
                var y = x_y.Item2;
                if (comparer.Compare(x, y) != 0)
                    return false;
            }
            return true;
        }

        public static void Sets()
        {
            {
                IEnumerable<int> xs = new int[] { 4, 2, 1, 5, 7, 2, 1, 2 };
                IEnumerable<int> ys = new int[] { 8, 1, 2, 4, 9, 1, 2, 4 };

                PrintLn(xs);
                PrintLn(ys);

                var xs_intersect_ys = xs.Intersect(ys).ToArray();
                PrintLn(xs_intersect_ys);

                var ys_intersect_xs = ys.Intersect(xs).ToArray();
                PrintLn(ys_intersect_xs);

                Console.WriteLine(Equals(xs_intersect_ys, ys_intersect_xs, new Int32Comparer()));
            }
            //Set Intersection requires Order

            {
                IEnumerable<int> xs = new int[] { 4, 2, 1, 5, 7, 2, 1, 2 };
                IEnumerable<int> ys = new int[] { 8, 1, 2, 4, 9, 1, 2, 4 };

                PrintLn(xs);
                PrintLn(ys);

                var xs_intersect_ys = new SortedSet<int>(xs);
                var ys_intersect_xs = new SortedSet<int>(ys);

                xs_intersect_ys.IntersectWith(ys);
                PrintLn(xs_intersect_ys);

                ys_intersect_xs.IntersectWith(xs);
                PrintLn(ys_intersect_xs);

                Console.WriteLine(Equals(xs_intersect_ys, ys_intersect_xs, new Int32Comparer()));
            }
        }

        public static void CartesianProduct()
        {
            char[] chars = new char[] { 'a', 'b', 'c' };
            int[] nums = new int[] { 1, 2, 3 };
            var chars_cross_nums = chars.SelectMany(c => nums.Select(n => Tuple.Create(c, n))).ToArray();
            var nums_cross_chars = nums.SelectMany(n => chars.Select(c => Tuple.Create(n, c))).ToArray();
            PrintLn(chars_cross_nums);
            PrintLn(nums_cross_chars);
        }
        #endregion

        #region "Relations"
        public static void Relation()
        {
            string[] Domain = new string[] { "Mark Twain", "Lewis Carroll", "Charles Dickens", "Stephen King" };
            string[] CoDomain = new string[] { "A Christmas Carol", "Alice's Adventures in Wonderland", "The Adventures of Tom Sawyer", "The Left Hand of Darkness" };
            PrintLn(Domain);
            PrintLn(CoDomain);

            Dictionary<string, string> R = new Dictionary<string, string>();
            R.Add(Domain[0], CoDomain[2]);
            R.Add(Domain[1], CoDomain[1]);
            R.Add(Domain[2], CoDomain[0]);
            PrintLn(R);

            var Image = CoDomain.Intersect(R.Values).ToArray();
            var PreImage = Domain.Intersect(R.Keys).ToArray();
            PrintLn(Image);
            PrintLn(PreImage);

            Dictionary<string, string> R_inverse = R.Select(kvp => kvp.Transpose()).ToDictionary();
            PrintLn(R_inverse);
        }
        #endregion

        #region "Functions"
        public static byte /* CoDomain */ duplicate(byte /* Domain */ x)
        {
            byte y = (byte)(2 * x /* pre-image */);
            return y /* image */;
        }

        public static byte duplicate(byte x, IEnumerable<byte> Domain)
        {
            byte y = (byte)((2 * x) % Domain.Count()); //modular arithmetic
            return y;
        }

        public static void Functions()
        {
            Console.WriteLine(duplicate(128));

            int start = Byte.MinValue;
            int end = Byte.MaxValue - Byte.MinValue;
            IEnumerable<byte> System_Byte_Domain = Enumerable.Range(start, end).Select(m => (byte)m);
            HashSet<byte> Image = new HashSet<byte>();
            foreach (var image in System_Byte_Domain.Select(x => duplicate(x)))
                Image.Add(image);
            PrintLn(Image);

            IEnumerable<byte> System_Byte_PreImage = System_Byte_Domain.Take(5);
            HashSet<byte> Image2 = new HashSet<byte>();
            foreach (var image in System_Byte_PreImage.Select(x => duplicate(x, System_Byte_PreImage)))
                Image2.Add(image);
            PrintLn(System_Byte_PreImage);
            PrintLn(Image2);
        }
        #endregion

        #region "Persistent and Non-Persistent Sets"
        //ImmutableUnion is commutative
        public static IList<Tuple<string, string>> ImmutableUnion(IList<Tuple<string, string>> contacts, Tuple<string, string> contact)
        {
            return contacts.Union(new Tuple<string, string>[] { contact }).ToList();
        }

        //ImmutableUnion is commutative
        public static IList<Tuple<string, string>> ImmutableUnion(Tuple<string, string> contact, IList<Tuple<string, string>> contacts)
        {
            return ImmutableUnion(contacts, contact);
        }

        //MutableUnion is commutative
        public static void MutableUnion(IList<Tuple<string, string>> contacts, Tuple<string, string> contact)
        {
            if (!contacts.Contains(contact))
                contacts.Add(contact);
        }

        //MutableUnion is commutative
        public static void MutableUnion(Tuple<string, string> contact, IList<Tuple<string, string>> contacts)
        {
            MutableUnion(contacts, contact);
        }

        public static void MoreSets()
        {
            var emptyContacts = new List<Tuple<string, string>>();

            var domain1 = ImmutableUnion(emptyContacts, Tuple.Create("xyz", "xyz@xyz.com"));
            var domain2 = ImmutableUnion(domain1, Tuple.Create("abc", "abc@abc.com"));
            var domain3 = ImmutableUnion(domain2, Tuple.Create("hello", "hello@world.com"));
            var domain4 = ImmutableUnion(Tuple.Create("hello", "hello@world.com"), domain3);
            PrintLn(emptyContacts);
            PrintLn(domain1);
            PrintLn(domain2);
            PrintLn(domain3);
            PrintLn(domain4);

            domain1[0] = Tuple.Create("ops", "ops@ops.com");
            PrintLn(domain1);
            PrintLn(domain2);

            var contacts = new List<Tuple<string, string>>();
            MutableUnion(contacts, Tuple.Create("xyz", "xyz@xyz.com"));
            MutableUnion(contacts, Tuple.Create("abc", "abc@abc.com"));
            MutableUnion(contacts, Tuple.Create("hello", "hello@world.com"));
            MutableUnion(Tuple.Create("hello", "hello@world.com"), contacts);
            PrintLn(contacts);

            contacts[1] = Tuple.Create("ops", "ops@ops.com");
            PrintLn(contacts);
        }
        #endregion

        #region "Function composition"
        //Compose isn't commutative
        public static Func<T, V> Compose<T, U, V>(Func<U, V> g, Func<T, U> f)
        {
            return x => g(f(x));
        }

        public static void Composition()
        {
            Func<int, int> f = x => 2 * x;
            Func<int, int> g = x => x + 1;
            {
                //h = g . f
                var h = Compose(g, f);
                Console.WriteLine(h(1));
                Console.WriteLine(h(2));
            }
            {
                //h = f . g
                var h = Compose(f, g);
                Console.WriteLine(h(1));
                Console.WriteLine(h(2));
            }
            {
                //f . (f . g)
                var h = Compose(f, Compose(f, g));
                //(f . f) . g
                var i = Compose(Compose(f, f), g);
                bool isAssociative = h(10) == i(10);
                Console.WriteLine("Is function composition an associative binary operation? " + isAssociative);
            }
            //function composition is associative
        }
        #endregion

        #region "Inverse functions"
        #region "Inverse function 2"
        [DataContract]
        sealed class Msg
        {
            public Msg(string a, int b, double c)
            {
                A = a; B = b; C = c;
            }

            [DataMember]
            public string A { get; set; }
            [DataMember]
            public int B { get; set; }
            [DataMember]
            public double C { get; set; }

            public override string ToString()
            {
                return String.Format("{{A: \"{0}\", B: {1}, C: {2}}}", A, B, C);
            }
        }
        #endregion

        public static void InverseFunctions()
        {
            #region "Inverse function 1"
            Func<double, double> lg = x => Math.Log(x, 2.0);
            Func<double, double> powerOf2 = x => Math.Pow(2.0, x);
            Console.WriteLine(lg(8.0));
            Console.WriteLine(powerOf2(3.0));
            Console.WriteLine(Compose(powerOf2, lg)(8.0));
            Console.WriteLine(Compose(lg, powerOf2)(3.0));
            #endregion

            #region "Inverse function 2"
            var msg0 = new Msg(a: "Hello World", b: 123, c: 456.789);
            Console.WriteLine(msg0);

            var json0 = SerializationUtil.SerializeToJson(msg0);
            Console.WriteLine(json0);

            var msg1 = SerializationUtil.DeserializeFromJson<Msg>(json0);
            Console.WriteLine(msg1);

            Func<string, Msg> g = x => SerializationUtil.DeserializeFromJson<Msg>(x);
            Func<Msg, string> f = x => SerializationUtil.SerializeToJson(x);
            var msg2 = Compose(g, f)(msg0);
            Console.WriteLine(msg2);
            #endregion
        }
        #endregion

        static void Main(string[] args)
        {
            Run(Sets, "Sets");
            Run(CartesianProduct, "CartesianProduct");
            Run(Relation, "Relation");
            Run(Functions, "Functions");
            Run(MoreSets, "MoreSets");
            Run(Composition, "Composition");
            Run(InverseFunctions, "InverseFunctions");

            Console.ReadLine();
        }
    }
}
