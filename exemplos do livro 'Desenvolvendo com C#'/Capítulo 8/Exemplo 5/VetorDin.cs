//csc VetorDin.cs

using System;
using System.Collections;

public class VetorDin
{
  public static void Main()
  {
    ArrayList arr = new ArrayList();
    arr.Add("C#");
    arr.Add("C++");
    arr.Add("Perl");
    arr.Add("Visual Basic");
    int a = arr.Count; // 4
    arr.RemoveAt(0); //Remove da 1ª posição
    arr.Remove("Perl");
    a = arr.Count; // 2
  } 
}
