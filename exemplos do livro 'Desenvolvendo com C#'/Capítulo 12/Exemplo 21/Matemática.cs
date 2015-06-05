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

  public double Soma(double x, double y)
  {
    return x + y; 
  }
  
  public double Soma(double x, double y, double z)
  {
    return this.Soma(x, y) + z; //double Soma(double, double)
  }

  public static void Main()
  {
    Matemática somador = new Matemática();
    somador.Soma(100, 200);            //int Soma(int, int)
    somador.Soma(100, 200, 300);       //int Soma(int, int, int)
    somador.Soma(100.1, 200.1);        //double Soma(double, double)
    somador.Soma(100.1, 200.1, 300.1); //double Soma(double, double, double)
  }
} 
