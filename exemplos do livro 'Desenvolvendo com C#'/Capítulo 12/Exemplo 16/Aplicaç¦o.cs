//csc Aplica��o.cs

using System;

public class MinhaClasseBase
{
  public int X, Y;
  
  public MinhaClasseBase(int x, int y)
  { 
    X = x; Y = y; 
  }  
} 

public class MinhaClasseDerivada : MinhaClasseBase
{
  public int Z;

  public MinhaClasseDerivada(int x, int y, int z) : base(x, y)
  { 
    Z = z; 
  }  
}

public class Aplica��o
{
  public static void Main()
  {
    MinhaClasseBase Base = new MinhaClasseDerivada(10, 20, 30);
   
    Console.WriteLine(Base.X);
    Console.WriteLine(Base.Y);
    //Console.WriteLine(Base.Z); //??? � A vari�vel Base n�o tem acesso  
    
    MinhaClasseDerivada Derivada = (MinhaClasseDerivada)Base; //Downcast
    
    Console.WriteLine(Derivada.X);
    Console.WriteLine(Derivada.Y);
    Console.WriteLine(Derivada.Z);
  }
}
