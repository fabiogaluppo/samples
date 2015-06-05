//csc Excecao.cs

using System;

public class Excecao
{
  public static void Main()
  {
    try
    {
      PreencherPilha();
    }
    catch(Exception e)
    {
      //Utilizando os membros da classe Exception
      Console.WriteLine("Exception Members");
      e.Source = "internal PreencherPilha Function";
      Console.WriteLine("Source: {0}", e.Source);
      Console.WriteLine("Message: {0}", e.Message);
      e.HelpLink = @"C:\Microsoft.Net\FrameworkSDK\Docs\cpref.chm";
      Console.WriteLine("HelpLink: {0}", e.HelpLink);
      Console.WriteLine("StackTrace: {0}", e.StackTrace);
      System.Reflection.MethodBase mb = e.TargetSite;
      if(mb.IsStatic) Console.Write("Membro que disparou a exceção é static");
    } 
  }

  internal static void PreencherPilha()
  {
    //Simulando estouro de pilha;
    throw new StackOverflowException();
  }
}
