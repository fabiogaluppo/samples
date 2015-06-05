//csc Vetores.cs

using System;

public class Unidimensional
{
  string[] s = {"um", "dois", "três", "quatro"};
  
  public void Processar()
  {
    Console.Write("\nUnidimensional = ");
    for(int idx = 0, l = s.Length; idx < l; ++idx)
      Console.Write("{0} ", s[idx]);
  }
}

public class Multidimensional
{
  string[,] s = { {"um", "dois"}, {"três", "quatro"} };
  
  public void Processar()
  {
    Console.Write("\nMultidimensional = ");
    for(int idx = 0, l = s.GetLength(0); idx < l; ++idx)
      for(int idx2 = 0, l2 = s.GetLength(1); idx2 < l2; ++idx2)  
        Console.Write("{0} ", s[idx, idx2]);   
  }
}

public class Jagged
{
  string[][] s = { new string[]{"um"}, new string[]{"dois", "três", "quatro"} };
  
  public void Processar()
  {
    Console.Write("\nJagged = ");
    for(int idx = 0, l = s.GetLength(0); idx < l; ++idx)
      for(int idx2 = 0, l2 = s[idx].GetLength(0); idx2 < l2; ++idx2)
        Console.Write("{0} ", s[idx][idx2]);
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
