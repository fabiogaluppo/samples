//csc /target:module Vetor.cs 

public class MuitosValoresVetor
{
  public void Exibir(int[] args)
  {
    for(int a = 0, l = args.Length; a < l; ++a)
    {
       System.Console.Write("{0} ", args[a]);
    }
    System.Console.WriteLine(); 
  }
} 
