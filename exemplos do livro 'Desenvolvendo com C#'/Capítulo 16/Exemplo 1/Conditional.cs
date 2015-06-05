//csc Conditional.cs
//csc Conditional.cs /define:EXECUTE

using System;
using System.Diagnostics;

public class MinhaClasse
{
  [Conditional("EXECUTE")]
  public void ProcessByCondition()
  {
    this.Processed = true;
  }

  public bool Processed;

  public static void Main()
  {
    MinhaClasse mc = new MinhaClasse();

    mc.ProcessByCondition();

    if(mc.Processed)
      Console.WriteLine("Símbolo encontrado");
    else
      Console.WriteLine("Símbolo não encontrado"); 
  }
}