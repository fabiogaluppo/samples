//csc SextaVers�o.cs

//6� vers�o
public class MinhaClasseBase
{
  public virtual string M�todo1()
  {
    return "MinhaClasseBase � M�todo 1";
  }
}

public class MinhaClasseDerivada : MinhaClasseBase
{
  public virtual void M�todo1()
  {
    System.Console.WriteLine("MinhaClasseDerivada � M�todo 1");
  }

  public static void Main()
  {
    MinhaClasseDerivada c = new MinhaClasseDerivada();

    c.M�todo1(); //"MinhaClasseDerivada � M�todo 1"
  }
}

