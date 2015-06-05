//csc Rethrow.cs

using System;

class Rethrow
{
  static int[] arr = new int[3]; //Declara um array de inteiros de 3 posições 

  public static void Main()
  {
    try
    {
      Func1();
    }
    catch(Exception) //Trata o rethrowing
    {	
      Console.WriteLine("O índice máximo é {0}", arr.GetUpperBound(0));		
    }
  }

  public static void Func1()
 {
   try
   {
     Func2();
   }    
   catch(IndexOutOfRangeException e) //Trata a exceção gerada
   {      
     Console.WriteLine(e); 
     throw e; //Rethrowing
   }
 }
  
  public static void Func2(){ arr[3] = 100; } //Exceção IndexOutOfRangeException
}
