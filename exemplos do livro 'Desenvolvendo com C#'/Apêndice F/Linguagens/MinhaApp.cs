//csc MinhaApp.cs /r:MinhaClasseCS.dll /r:MinhaClasseVB.dll /r:MinhaClasseCPP.dll

public class MinhaClasseCSEx : MinhaClasseCPP
{
  public void Metodo4()
  {
    System.Console.WriteLine("C#");
  }
}

public class MinhaApp
{
  public static void Main()
  {
    MinhaClasseCSEx c = new MinhaClasseCSEx();

    c.Metodo1(); //C#
    c.Metodo2(); //VB.NET
    c.Metodo3(); //VC++.NET
    c.Metodo4(); //C#
  }
}