//csc Matemática.cs

public class Matemática
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

  public static void Main()
  {
    Matemática somador = new Matemática();
    somador.Soma(100, 200);            //int Soma(int, int)
    somador.Soma(100, 200, 300);       //int Soma(int, int, int)
    somador.Soma(100.1, 200.1);        //double Soma(double, double)
    somador.Soma(100.1, 200.1, 300.1); //double Soma(double, double, double)
    somador.Soma(1, 2, 3, 4);          //int Soma(params int[])
    somador.Soma(1, 2, 3, 4, 5);       //int Soma(params int[])
    somador.Soma(1d, 2d, 3d, 4d);      //double Soma(params double[])
    somador.Soma(1, 2, 3d, 4, 5);      //double Soma(params double[]) //;) trick
  }
} 
