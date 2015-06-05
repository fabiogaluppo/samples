//csc LinhaDeComando.cs

using System;

class LinhaDeComando
{
  static void Main(string[] args)
  {
    int a = 0;
    if(args.Length < 1) return; //Se o número de argumentos for menor que 1, fim
    Console.WriteLine("N. de argumentos = {0}", args.Length);
    foreach(string arg in args) 
      Console.WriteLine("Argumento {0} = {1}", a++, arg);
  }
}
