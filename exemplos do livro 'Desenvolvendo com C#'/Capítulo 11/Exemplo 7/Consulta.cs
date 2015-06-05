//csc /target:exe Consulta.cs /r:Hóspede.dll
 
using System;
using System.Text.RegularExpressions;
using System.Xml;

public class Consulta
{
  public static void Main(string[] args)
  {
    if(args.Length == 1)
    {
      Regex r = new Regex(@"(\/|\\|\-)(?<param>(\S+))\:(?<valor>(\w+\@\S+))");
      Match m = r.Match(args[0]);
      
      if(m.Length != 0)
      {        
        if(m.Groups["param"].Value == "email")
        {
	  HóspedeSimpleFinder hsf = new HóspedeSimpleFinder();
          Hóspede h = hsf.ProcurarPeloEmail(m.Groups["valor"].Value);
          if(h != null)
          {
            string fmt = "Hóspede {0}: {1} {2} - {3}";
            Console.WriteLine(fmt, h.ID, h.Nome, h.Sobrenome, h.Email);
          }
          else
          {
            Console.WriteLine("Hóspede não encontrado");
          }
        }
      }
      else
      {
        ModoUsar();
      } 
    }
    else
    {
      ModoUsar();
    } 
  }

  private static void ModoUsar()
  {
    Console.WriteLine(@"Modo de usar: Consulta </|\|->email:<email>");
    Console.WriteLine(@"Exemplo: Consulta /email:fabiogaluppo@hotmail.com");
  }  
}