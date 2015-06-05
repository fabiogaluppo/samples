//csc LinhaDeComando.cs /main:LinhaDeComando1
//csc LinhaDeComando.cs /main:LinhaDeComando2

using System;

class LinhaDeComando1
{
  static void Main(string[] args)
  {
    Console.WriteLine("N. de argumentos = {0}", args.Length);
    for(int a = 0; a < args.Length; ++a) 
      Console.WriteLine("Argumento {0} = {1}", a, args[a]);    
  }
}

class LinhaDeComando2
{
  static void Main(string[] args)
  {
    int a = 0;
    Console.WriteLine("N. de argumentos = {0}", args.Length);
    foreach(string arg in args) 
      Console.WriteLine("Argumento {0} = {1}", a++, arg);
  }
}
