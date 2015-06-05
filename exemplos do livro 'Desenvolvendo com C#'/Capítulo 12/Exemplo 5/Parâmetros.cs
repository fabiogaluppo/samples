//csc Parâmetros.cs /addmodule:Vetor.netmodule /addmodule:Params.netmodule 

public class Parâmetros
{
  public static void Main()
  {
    MuitosValoresVetor v = new MuitosValoresVetor();
  
    //vetor

    int[] a = {1, 2, 3, 4, 5, 6};

    v.Exibir(a);

    int[] b = {int.MinValue, int.MaxValue};

    v.Exibir(b);

    //params
  
    MuitosValoresParams p = new MuitosValoresParams();

    p.Exibir(1, 2, 3, 4, 5, 6, 7);

    p.Exibir(int.MinValue, int.MaxValue); 
  }
}
