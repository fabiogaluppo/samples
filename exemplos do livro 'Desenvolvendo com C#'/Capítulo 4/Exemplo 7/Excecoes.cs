//csc Excecoes.cs

using System;

public class Excecoes
{
  public static void Main()
  {
    try
    {
      try
      {	
        Console.WriteLine("Bloco try");
        throw new NullReferenceException();    
      }
      catch(DivideByZeroException e)
      {
        Console.WriteLine("Bloco catch #1. Mensagem: {0}",e.Message);
      }     
      catch(NullReferenceException e)
      {
        Console.WriteLine("Bloco catch #2. Mensagem: {0}",e.Message);
      }
      catch(Exception e)
      {
        Console.WriteLine("Bloco catch #3. Mensagem: {0}",e.Message);
      }
    }     
    finally
    {    
      Console.WriteLine("Bloco finally");
    }    
  }
}
