//csc QuartaVersão.cs

//4ª versão
public class MinhaClasseBase
{
  public virtual void Método1()
  {
    System.Console.WriteLine("MinhaClasseBase – Método 1");
  }
}

public class MinhaClasseDerivada : MinhaClasseBase
{
  public new virtual void Método1()
  {
    System.Console.WriteLine("MinhaClasseDerivada – Método 1");
  }

  public static void Main()
  {
    MinhaClasseBase c = new MinhaClasseDerivada();

    c.Método1(); //"MinhaClasseBase – Método 1"
  }
}

