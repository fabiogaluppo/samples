//csc Excecao.cs

using System;

public class Excecao
{
  public static void Main()
  { 
    try
    {	
      throw new Exception("A exce��o..."); //Dispara a exce��o
    }
    finally
    {
      Console.WriteLine("O bloco finally � sempre executado..."); 
    }

    Console.WriteLine("Esta linha n�o ser� executada...");
  }
}
