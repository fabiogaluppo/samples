//csc Encapsulamento.cs /r:InputBox.dll

using System;
using CSharp;

public class Encapsulamento
{
  public static void Main()
  {
    string msg = "Usando a InputBox sem se preocupar com os detalhes";
    string tit = "Encapsulamento";
    
    string s = InputBox.Show(msg, tit);

    Console.Write(s);
  }
}
