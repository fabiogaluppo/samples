//csc /t:library Classe.cs /r:System.dll
 
using System;

public class MinhaClasse
{
  public long valor = 100; //campo

  public const long capacidade_buffer = (long.MaxValue / 2) + 100L; //const

  public readonly int seed = 222; //readonly (tempo de compilação)

  public readonly DateTime hoje = DateTime.Now; //readonly (tempo de execução)
}
