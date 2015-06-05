//csc MethodSafe.cs

using System;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class StackEx : Stack
{
  public StackEx(int capacity)
  {
    Capacity = capacity;
  }  
  
  public readonly int Capacity; 
}

public class SafeSample
{
  StackEx stack = new StackEx(20);
 
  [MethodImpl(MethodImplOptions.Synchronized)]
  public void WorkerThread1()
  {
    string methodname = GetMethodName();
 
    for(int a = 0, l = stack.Capacity; a < l; ++a)
    {
      stack.Push(a + 1);
      Display(methodname);
      Thread.Sleep(300);
    }
  }

  [MethodImpl(MethodImplOptions.Synchronized)]
  public void WorkerThread2()
  {
    string methodname = GetMethodName();

    Display(methodname);
    for(int a = 0, l = stack.Count - 1; a < l; ++a)
    {
      stack.Pop();
      Display(methodname);
      Thread.Sleep(300);      
    }
    stack.Pop();
  }

  public void WorkerThread3()
  {
    string methodname = GetMethodName();

    while(stack.Count > 0)
    {
      Display(methodname);
      Thread.Sleep(1200);
    }
  }    
  
  public void Display(string sender)
  {
    Console.WriteLine("{1} - Value = {0}", stack.Peek(), sender);
  }
  
  private string GetMethodName()
  {
    StackTrace stacktrace = new StackTrace();
    StackFrame stackframe = stacktrace.GetFrame(0); 
    return stackframe.GetMethod().Name;
  }   
  
  public static void Main()
  {
    SafeSample o = new SafeSample();
    
    Thread t1 = new Thread(new ThreadStart(o.WorkerThread1));
    Thread t2 = new Thread(new ThreadStart(o.WorkerThread2));
    Thread t3 = new Thread(new ThreadStart(o.WorkerThread3));

    t1.Start();
    t2.Start();
    t3.Start();
  }
}