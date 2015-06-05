//csc Dicion�rio.cs 

using System.Collections;

public class Dicion�rio
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
    Dicion�rio dic = new Dicion�rio();

    dic["for"] = "para";
    dic["while"] = "enquanto";
    dic["if"] = "se";

    System.Console.WriteLine(dic["while"]);
    System.Console.WriteLine(dic["switch"]);  
  }
}