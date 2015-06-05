//csc Triângulo.cs

public class Catetos
{
  public double oposto, adjacente;
}
 
public class Triângulo : Catetos
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

public class Aplicação
{
  public static void Main(string[] args)
  {
    Triângulo triângulo = new Triângulo();
    Catetos cateto = (Catetos)triângulo;
    cateto.oposto = System.Int32.Parse(args[0]);
    cateto.adjacente = System.Int32.Parse(args[1]);
    
    System.Console.WriteLine(triângulo.hipotenusa);   
  }
} 
