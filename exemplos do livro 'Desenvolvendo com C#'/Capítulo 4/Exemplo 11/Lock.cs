//csc Lock.cs

using System;
using System.Threading;

class Lock
{
  static int x = 0;
  
  public static void ThreadProc() //Thread Procedure
  {
    lock(typeof(Lock)) //Para evitar acesso concorrente a váriavel x
    {
      x++;        
    }

    Console.WriteLine("x = {0}",x);
  }

  public static void Main()
  {
    for(int a=0; a < 10; ++a)
    {
      Thread t = new Thread(new ThreadStart(ThreadProc)); //Cria uma thread
      t.Start(); //Inicializa execução da thread
    }            
  }
}
