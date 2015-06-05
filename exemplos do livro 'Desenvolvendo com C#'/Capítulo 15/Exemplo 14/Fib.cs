//csc Fib.cs

using System;
using System.Runtime.CompilerServices;

public class Fibonacci 
{
  [MethodImpl(MethodImplOptions.NoInlining)]
  public void Next(ref int a, ref int b) 
  {
    int temp = a + b;
    a = b;
    b = temp;
  }	
   	
  public void Process(int max)
  {
    int fib1 = 1, fib2 = 1;

    for (int a = 0; a < max; a++) Next(ref fib1, ref fib2);    
  }
}

public class MinhaAplicação
{
  public static void Main()
  {
    Fibonacci fib = new Fibonacci();

    fib.Process(10);
  }  
}


