//csc PrimeiraVersão.cs

//1ª versão
public class MinhaClasseBase
{
}

public class MinhaClasseDerivada : MinhaClasseBase
{
  public virtual void Método1()
  {
    System.Console.WriteLine("MinhaClasseDerivada – Método 1");
  }

  public static void Main()
  {
    MinhaClasseDerivada c = new MinhaClasseDerivada();

    c.Método1(); //"MinhaClasseDerivada – Método 1"
  }
}

