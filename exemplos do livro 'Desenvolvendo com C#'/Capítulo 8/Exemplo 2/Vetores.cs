//csc Vetores.cs

using System;

public class Unidimensional
{
  string[] s = {"um", "dois", "três", "quatro"};
  
  public void Processar()
  {
    Console.Write("\nUnidimensional = ");
    foreach(string e in s)
      Console.Write("{0} ", e);
  }
}

public class Multidimensional
{
  string[,] s = { {"um", "dois"}, {"três", "quatro"} };
  
  public void Processar()
  {
    Console.Write("\nMultidimensional = ");
    foreach(string e in s)
      Console.Write("{0} ", e);   
  }
}

public class Jagged
{
  string[][] s = { new string[]{"um"}, new string[]{"dois", "três", "quatro"} };
  
  public void Processar()
  {
    Console.Write("\nJagged = ");
    for(int idx = 0, l = s.GetLength(0); idx < l; ++idx)
    foreach(string e in s[idx])
      Console.Write("{0} ", e);
  }
}

public class Vetores
{
  public static void Main()
  {
    Unidimensional uni = new Unidimensional();
    Multidimensional multi = new Multidimensional();
    Jagged jag = new Jagged();

    uni.Processar();
    multi.Processar();
    jag.Processar();
  }
}  
