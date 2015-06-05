//csc Dicionário.cs 

using System.Collections;

public class Dicionário
{
  private Hashtable h = new Hashtable(10);
  
  public string this[string palavrachave]
  {
    get
    {
      if(h.Contains(palavrachave)) 
        return (string)h[palavrachave];
      return null;
    }

    set
    {
      h[palavrachave] = value;
    }
  }

  public static void Main()
  {
    Dicionário dic = new Dicionário();

    dic["for"] = "para";
    dic["while"] = "enquanto";
    dic["if"] = "se";

    System.Console.WriteLine(dic["while"]);
    System.Console.WriteLine(dic["switch"]);  
  }
}