//csc Async.cs

using System;

public class AsyncClass
{
  public delegate int AsyncFunc(int a, int b, int c);
 
  public int LongRunningFunc(int a, int b, int c)
  {
    Console.WriteLine("Processando"); 
    
    System.Threading.Thread.Sleep(a * b * c);
  
    return a + b + c;
  }

  public void FuncCallback(IAsyncResult iar)
  {
    Console.WriteLine("Fim de processamento");

    AsyncFunc af = (AsyncFunc)iar.AsyncState;
    int retorno = af.EndInvoke(iar);
    
    Console.WriteLine("Resultado = {0}", retorno);  
  }

  public static void Main()
  {
    AsyncClass ao = new AsyncClass();

    AsyncFunc af = new AsyncFunc(ao.LongRunningFunc);

    AsyncCallback cb = new AsyncCallback(ao.FuncCallback);

    IAsyncResult iar = af.BeginInvoke(10, 20, 30, cb, af);

    Console.WriteLine("Inicializando processamento");

    while(!iar.IsCompleted);
  }
}