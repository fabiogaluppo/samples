//csc /out:Event.exe EfficientEvent.cs /incremental+ /optimize+ /debug:full

using System;
using System.Collections;

public class Fibonacci 
{
  public Fibonacci()
  {
    key = this.GetHashCode();
  }

  public void Next(ref int a, ref int b) 
  {
    int temp = a + b;
    a = b;
    b = temp;
  }	
   	
  public void Process(int max)
  {
    int fib1 = 1, fib2 = 1;
    
    for (int a = 0; a < max; a++) 
    {
      OnCalculating(fib1, fib2);

      Next(ref fib1, ref fib2);
    }
    OnCalculating(fib1, fib2);
  }

  protected void OnCalculating(int a, int b)
  {
    CalculatingHandler ch = (CalculatingHandler)storage[key];
    
    if(ch != null) ch(a, b); 
  }

  private Hashtable storage = new Hashtable();   
  private int key;
  
  public delegate void CalculatingHandler(int a, int b);  

  public event CalculatingHandler Calculating
  {
    add
    {
      storage[key] = Delegate.Combine((Delegate)storage[key], value);
    }

    remove
    {
      storage[key] = Delegate.Remove((Delegate)storage[key], value);   
    }
  }   
}

public class MinhaAplicação
{
  public static void Calculating(int a, int b)
  {
    Console.WriteLine("{0} {1}", a, b);
  }

  public static void Main()
  {
    //Console.ReadLine(); 
   
    Fibonacci fib = new Fibonacci();

    fib.Calculating += new Fibonacci.CalculatingHandler(MinhaAplicação.Calculating);

    fib.Process(10);

    fib.Calculating -= new Fibonacci.CalculatingHandler(MinhaAplicação.Calculating);
  }  
}


