//csc ProgramaChamador.cs

using System;
using System.Diagnostics;

class ProgramaChamador
{
  static void Main()
  {    
    Process p1 = Process.Start("ProgramaChamado1.exe");
    p1.WaitForExit();
    Console.WriteLine("O ProgramaChamado1 retornou {0}", p1.ExitCode);    
    
    Process p2 = Process.Start("ProgramaChamado2.exe");
    p2.WaitForExit();
    Console.WriteLine("O ProgramaChamado2 retornou {0}", p2.ExitCode);
  }
}