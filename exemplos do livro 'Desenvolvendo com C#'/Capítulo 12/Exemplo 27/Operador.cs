//csc Operador.cs

public class Coordenada
{
  public int X, Y;
  
  //operador unário
  public static Coordenada operator-(Coordenada operando)
  {
    operando.X *= -1;
    operando.Y *= -1;
    return  operando;
  }

  public static Coordenada operator+(Coordenada lhs, Coordenada rhs)
  {
    Coordenada c = new Coordenada();
    c.X = lhs.X + rhs.X;
    c.Y = lhs.Y + rhs.Y;
    return c;
  }

  public static void Main()
  {
    Coordenada c1 = new Coordenada();
    Coordenada c2 = new Coordenada();

    c1.X = 100;
    c1.Y = 100; 
    c2.X = 150;
    c2.Y = 150;

    c1 = -c1;
    System.Console.WriteLine("X = {0}; Y = {1}", c1.X, c1.Y);

    Coordenada c3 = c1 + c2;
    System.Console.WriteLine("X = {0}; Y = {1}", c3.X, c3.Y);
  }
}   
