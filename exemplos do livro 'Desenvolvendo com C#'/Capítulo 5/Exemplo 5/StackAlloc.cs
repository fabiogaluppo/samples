//csc StackAlloc.cs /unsafe+

using System;

public class StackAlloc
{    
  public static void Main(string[] args)
  {   
    int count = Convert.ToInt32(args[0]);   
    
    unsafe
    {      
      int* pi = stackalloc int[count];
      for(int a = 0; a < count; ++a) pi[a] = a + 1;
      for(int a = 0; a < count; ++a, ++pi) Console.WriteLine(*pi); 
    }
  }
}
