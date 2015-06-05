//MatEsp.cs

public class MatEsp
{
  public int Soma(int x, int y)
  {
    return x + y;
  }

  public int Soma(int x, int y, int z)
  {
    return this.Soma(x, y) + z; //int Soma(int, int)
  }
  
  public int Soma(params int[] valores)
  {
    int resultado = 0;

    foreach(int i in valores)
    {
      resultado += i; 
    }
    
    return resultado;
  }

  public double Soma(double x, double y)
  {
    return x + y;
  }
  
  public double Soma(double x, double y, double z)
  {
    return this.Soma(x, y) + z; //double Soma(double, double)
  }
  
  public double Soma(params double[] valores)
  {
    double resultado = 0d;

    foreach(double d in valores)
    {
      resultado += d; 
    }
    
    return resultado;
  }
} 
