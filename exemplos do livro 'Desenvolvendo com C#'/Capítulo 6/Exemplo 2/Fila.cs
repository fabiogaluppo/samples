//csc Fila.cs

using System;
using System.Collections;

public class Fila
{
  public static void Main()
  {
    Queue fila = new Queue(5);

    fila.Enqueue(1);   //int
    fila.Enqueue("2"); //string
    fila.Enqueue('3'); //char
    fila.Enqueue(4L);  //long
    fila.Enqueue(5F);  //double

    Console.WriteLine("Esta fila possui {0} elementos", fila.Count);

    for(int a = 0, l = fila.Count; a < l; ++a )
    {
      Console.WriteLine(fila.Dequeue().ToString());
    }
  }
}