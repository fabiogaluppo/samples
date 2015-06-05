//csc Ponteiro.cs /unsafe+

using System;

public class Pointeiro
{
  unsafe public static void Process(int[] a)
  {        
    fixed(int* pa = a)
    {
      for(int i=0; i < a.Length; ++i)      
        Console.Write("{0} ", *(pa+i));
    }
  }
  
  public static void Main()
  {
    int[] arr = {1,2,3,4,5,6,7,8,9,0};
     
    Process(arr);            
  }
}
