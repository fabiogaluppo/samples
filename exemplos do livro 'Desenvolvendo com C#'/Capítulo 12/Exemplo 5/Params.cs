//csc /target:module Params.cs

public class MuitosValoresParams
{
  public void Exibir(params int[] args)
  {
    for(int a = 0, l = args.Length; a < l; ++a)
    {
       System.Console.Write("{0} ", args[a]);
    }
    System.Console.WriteLine(); 
  }
}
