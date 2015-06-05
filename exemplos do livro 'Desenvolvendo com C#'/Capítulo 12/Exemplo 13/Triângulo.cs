//csc Tri�ngulo.cs

public class Catetos
{
  public double oposto, adjacente;
}
 
public class Tri�ngulo : Catetos
{
  public double hipotenusa
  {
    get
    {
      Hipotenusa h = new Hipotenusa();
      return h.Calcular(this);
    }
  }
}

public class Hipotenusa
{
  public double Calcular(Catetos cat)
  {
    double calc = cat.oposto * cat.oposto + cat.adjacente * cat.adjacente;
    return System.Math.Sqrt(calc); 
  }
}

public class Aplica��o
{
  public static void Main(string[] args)
  {
    Tri�ngulo tri�ngulo = new Tri�ngulo();
    Catetos cateto = (Catetos)tri�ngulo;
    cateto.oposto = System.Int32.Parse(args[0]);
    cateto.adjacente = System.Int32.Parse(args[1]);
    
    System.Console.WriteLine(tri�ngulo.hipotenusa);   
  }
} 
