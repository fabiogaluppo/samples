//csc Base.cs

using System;

public class MinhaClasseBase
{
  public void Exibir()
  {
    Console.WriteLine("método Exibir da classe base");
  }
}

public class MinhaClasseDerivada : MinhaClasseBase
{
  public new void Exibir()
  {
    base.Exibir(); 
    Console.WriteLine("método Exibir da classe : {0}", this.GetType().Name);  
  }
}

public class Aplicação
{
  public static void Main()
  {
    (new MinhaClasseDerivada()).Exibir();
  }
}
