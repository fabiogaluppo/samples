//csc LinhaDeComando.cs

using System;

public class LinhaDeComando
{
  public static void Main(string[] args)
  {
    //Verifica se somente uma string foi entrada
    if(args.Length==1)
      System.Console.WriteLine(args[0]);
    else      
      //Dispara a exceção
      throw(new ArgumentOutOfRangeException("Utilize uma string somente"));    
  }
}
