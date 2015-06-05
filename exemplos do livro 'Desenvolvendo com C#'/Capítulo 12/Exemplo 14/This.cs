//csc This.cs

public class MinhaClasse
{
  int X, Y, Z;

  public MinhaClasse(int x, int y, int z){ X = x; Y = y; Z = z; }
  
  public MinhaClasse(int x, int y) : this(x, y, 10){}
  
  public MinhaClasse(int x) : this(x, 15){}

  public MinhaClasse() : this(20){} 

  public static void ExibirCampos(MinhaClasse c)
  {
    System.Console.WriteLine("X = {0,2}; Y = {1,2}; Z = {2,2}", c.X, c.Y, c.Z); 
  }  

  public static void Main()
  {
    MinhaClasse c1 = new MinhaClasse();        //x = 20; y = 15; z = 10
    MinhaClasse c2 = new MinhaClasse(12);      //x = 12; y = 15; z = 10
    MinhaClasse c3 = new MinhaClasse(30, 40);  //x = 30; y = 40; z = 10
    MinhaClasse c4 = new MinhaClasse(1, 2, 3); //x =  1; y =  2; z =  3

    MinhaClasse.ExibirCampos(c1);
    MinhaClasse.ExibirCampos(c2);
    MinhaClasse.ExibirCampos(c3);
    MinhaClasse.ExibirCampos(c4);
  }
} 
