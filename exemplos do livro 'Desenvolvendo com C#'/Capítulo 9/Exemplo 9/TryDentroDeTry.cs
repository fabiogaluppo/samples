//csc TryDentroDeTry.cs

using System;

public class TryDentroDeTry 
{
  static int[] arr = new int[3];

  public static void Main()
  {
    try
    {
      Func1();
    }
    catch(IndexOutOfRangeException e)
    {
      Console.WriteLine(e);
      try
      {
        throw e;
      }
      catch(Exception)
      {	
        Console.WriteLine("O índice máximo é {0}", arr.GetUpperBound(0));		
      }    
    }  
  }

  public static void Func1(){ Func2(); }
  
  public static void Func2(){ arr[3] = 100; }
}
