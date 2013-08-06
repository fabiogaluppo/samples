//Sample provided by Fabio Galuppo
//August 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using SupportLibrary;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace _Parallel
{
    sealed class Person
    {
        private static string Normalize(string name)
        {
            var head = Char.ToUpper(name.Head());
            var tail = name.Tail().ToArray();
            return new StringBuilder(name.Length).Append(head).Append(tail).ToString();
        }
        
        public Person(string firstName, string lastName)
        {
            FullName = Normalize(firstName) + " " + Normalize(lastName);
        }

        public readonly string FullName;

        public string Key { get { return FullName; } }
    }

    class NamesProgram
    {
        static string[] LoadData(string path)
        {
            return File.ReadLines(path).Select(line => line).ToArray();
        }

        static void LoadAllData(out string[] maleNames, out string[] femaleNames, out string[] surnames)
        {
            maleNames = LoadData(@"..\..\..\..\files\surnames.txt");
            femaleNames = LoadData(@"..\..\..\..\files\female.txt");
            surnames = LoadData(@"..\..\..\..\files\male_names.txt");
        }

        public static void Run(params string[] args)
        {
            int peopleCount = 1000;
            if (args.Length > 0) Int32.TryParse(args[0], out peopleCount);

            string[] maleNames = null, femaleNames = null, surnames = null;
            var y = InstrumentedOperation.Test(() => LoadAllData(out maleNames, out femaleNames, out surnames), "LoadAllData");

            var allNames = (maleNames.Shuffle().Union(femaleNames.Shuffle()))
                                .ToArray()
                                .Shuffle()
                                .SelectMany(firstname => surnames.Select(lastname => Tuple.Create(firstname, lastname)))
                                .Select(name => new Person(name.Item1, name.Item2));


            peopleCount = 1000000;
            ConcurrentDictionary<string, Person> xs = new ConcurrentDictionary<string, Person>();
            InstrumentedOperation.Test(() => 
            {

                allNames.ToArray()
                        .Shuffle()
                        .Take(peopleCount)
                        .AsParallel()
                        .ForAll(p => xs.TryAdd(p.Key, p));

            }, "Adding people to the dictionary (data parallelism)");

            ConcurrentBag<Person> xs1 = new ConcurrentBag<Person>(), 
                                  xs2 = new ConcurrentBag<Person>();

            InstrumentedOperation.Test(() =>
            {
                    
                    Parallel.Invoke
                    (
                        () => Parallel.ForEach(xs, x =>
                        {
                            var p = x.Value;
                            if (p.FullName.Length > 12)
                                xs1.Add(p);
                        }),

                        () =>
                        {
                            xs.AsParallel()
                              .Select(x => x.Value)
                              .Where(p => p.FullName.StartsWith("A") || p.FullName.StartsWith("B"))
                              .ForAll(xs2.Add);
                        }
                    );

            }, "Filtering people from the dictionary (task parallelism)");
            ConsoleEx.WriteLn(xs1.Select(p => p.FullName).AsEnumerable(), 5, "fullname length greater than 12: ");
            ConsoleEx.WriteLn(xs2.Select(p => p.FullName).AsEnumerable(), 5, "fullname starts with A or B: ");
        }
    }
}
