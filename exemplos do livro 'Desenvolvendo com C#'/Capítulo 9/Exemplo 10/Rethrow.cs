//csc Rethrow.cs

using System;

public class Rethrow
{
  static int[] arr = new int[3]; //Declara um array de inteiros de 3 posi��es 

  public static void Main()
  {
    try
    {
      Func1();
    }
    catch(Exception e) //Trata o rethrowing
    {	
      Console.WriteLine(e);
      Console.WriteLine("O �ndice m�ximo � {0}", arr.GetUpperBound(0));		
    }
  }

  public static void Func1()
 {
   try
   {
     Func2();
   }    
   catch
   {      
     throw; //Rethrowing
   }
 }
  
  public static void Func2(){ arr[3] = 100; } //Exce��o IndexOutOfRangeException
}
