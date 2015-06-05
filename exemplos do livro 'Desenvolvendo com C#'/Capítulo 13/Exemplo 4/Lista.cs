//csc Lista.cs

public class Itens
{
  private string[] itens;
  private int n�merodeitens;
  
  public Itens(int capacidade)
  {
    itens = new string[capacidade];
    n�merodeitens = 0;
  }   

  public string this[int �ndice]
  {
    get
    {
      if(�ndice > -1 && �ndice < n�merodeitens)      
        return itens[�ndice];
      return null;
    }

    set
    {
      if(n�merodeitens < itens.Length)
      {
        itens[n�merodeitens] = value; n�merodeitens++;
      } 
    }
  }

  public static void Main()
  {
    Itens lista = new Itens(5);

    lista[0] = "Windows XP Professional";
    lista[1] = "Mac OS X 10.2";
    lista[2] = "Microsoft Office XP";
    lista[3] = "Eiffel ENViSioN! 1.0";
    lista[4] = "ActiveState ActivePerl 5.6";

    for(int a = 0; a < 5; ++a)
      System.Console.WriteLine(lista[a]);
  }
}