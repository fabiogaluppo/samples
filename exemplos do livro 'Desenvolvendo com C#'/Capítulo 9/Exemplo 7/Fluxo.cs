//csc Fluxo.cs

using System;

class Fluxo
{
  static int[] arr = new int[3]; //Declara um array de inteiros de 3 posi��es 

  public static void Main()
  {    
    try
    {
      Func1(); //Chama o m�todo Func1
    }
    catch //catch na forma isolada
    {
      Console.WriteLine("Ocorreu um erro no m�todo Main");
    }
  }

  public static void Func1()
  { 
    Func2(); //Chama o m�todo Func2 
  }

  public static void Func2(){ arr[3] = 100; }  
}
