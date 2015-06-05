//csc LinhaDeComando.cs

using System;

public class LinhaDeComando
{
  public static void Main(string[] args)
  {
    //Verifica se somente uma string foi entrada
    if(args.Length == 1)
      System.Console.WriteLine(args[0]);
    else
    {
      ArgumentOutOfRangeException ex; 
      ex = new ArgumentOutOfRangeException("Utilize uma string somente");
      throw(ex); //Dispara a exceção
    }
  }
}
