//csc Declaracoes.cs

using System;

public class Declaracoes
{  
  private static int f = 1000, g;        //Variáveis de escopo de classe  	
  private const int m = 1000, n = 10000; //Constantes de escopo de classe

  public static void Main()
  {
    //Constantes de escopo local
    const int  x = 10;
    const long y = 100;
	
    //Variáveis de escopo local
    int  a = 10;
    long b;

    b = 100;
    g = 10000;	

    Print(x, y, a, b);     	
  }

  //Função ou método
  private static void Print(int ix, long ly, int ia, long lb)
  {
    Console.WriteLine("Locais");
    Console.WriteLine("const: x={0} y={1} var: a={2} b={3}", ix, ly, ia, lb);
    Console.WriteLine("Classe");
    Console.WriteLine("const: m={0} n={1} var: f={2} g={3}" , m, n, f, g);
  }
}
