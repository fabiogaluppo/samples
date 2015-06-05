//csc Conversoes.cs

using System;

public class Conversoes
{
  public static void Main()
  {
    int x;
    long y = 10;

    Console.WriteLine("Cast\n----");

    //Cast consistente
    x = (int)y;
    Console.WriteLine("Consistente int x = {0} \u2192 long y = {1}", x, y);
  
    //Cast inconsistente
    y = 2147483648;
    x = (int)y;
    Console.WriteLine("Inconsistente int x = {0} \u2192 long y = {1}", x, y);  
  }
}

