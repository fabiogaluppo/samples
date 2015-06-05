//csc Credito.cs /r:Conta.dll

using System;

public class Credito
{
  public static void Main()
  {
    Conta c = new Conta();
    c.Creditar("Fabio Galuppo", 1000.55);
  }
}