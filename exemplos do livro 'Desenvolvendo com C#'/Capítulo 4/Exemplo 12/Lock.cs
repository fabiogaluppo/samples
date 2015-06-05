//csc Lock.cs

using System;
using System.Threading;

public class Lock
{
  int x = 0;
  
  public void ThreadProc() //Thread Procedure
  {
    lock(this) //Para evitar acesso concorrente a váriavel x
    {
      x++;        
    }

    Console.WriteLine("x = {0}",x);
  }

  public static void Main()
  {
    Lock _lock = new Lock();
 
    for(int a=0; a < 10; ++a)
    {
      //Cria thread t1
      Thread t1 = new Thread(new ThreadStart(_lock.ThreadProc)); 
      //Cria thread t2
      Thread t2 = new Thread(new ThreadStart(_lock.ThreadProc));
      t2.Start(); //Inicializa execução da thread t2
      t1.Start(); //Inicializa execução da thread t1     
    }            
  }
}
