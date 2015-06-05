//csc Excecao.cs

using System;

public class Excecao
{
  public static void Main()
  { 
    try
    {	
      throw new Exception("A exceção..."); //Dispara a exceção
    }
    finally
    {
      Console.WriteLine("O bloco finally é sempre executado..."); 
    }

    Console.WriteLine("Esta linha não será executada...");
  }
}
