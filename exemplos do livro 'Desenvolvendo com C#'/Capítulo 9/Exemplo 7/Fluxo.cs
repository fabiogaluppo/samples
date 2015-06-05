//csc Fluxo.cs

using System;

class Fluxo
{
  static int[] arr = new int[3]; //Declara um array de inteiros de 3 posições 

  public static void Main()
  {    
    try
    {
      Func1(); //Chama o método Func1
    }
    catch //catch na forma isolada
    {
      Console.WriteLine("Ocorreu um erro no método Main");
    }
  }

  public static void Func1()
  { 
    Func2(); //Chama o método Func2 
  }

  public static void Func2(){ arr[3] = 100; }  
}
