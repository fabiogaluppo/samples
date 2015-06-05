//csc QuintaVers�o.cs

//5� vers�o
public class MinhaClasseBase
{
  public virtual void M�todo1()
  {
    System.Console.WriteLine("MinhaClasseBase � M�todo 1");
  }
}

public class MinhaClasseDerivada : MinhaClasseBase
{
  public override void M�todo1()
  {
    System.Console.WriteLine("MinhaClasseDerivada � M�todo 1");
  }

  public static void Main()
  {
    MinhaClasseBase c = new MinhaClasseDerivada();

    c.M�todo1(); //"MinhaClasseDerivada � M�todo 1"
  }
}

