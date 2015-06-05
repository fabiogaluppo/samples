//csc Teste.cs

public class Teste   
{
  public class Pessoa{ public string Nome, Sobrenome; }
   
  public void Swap(ref string a, ref string b)
  {
    string temp = a;
    a = b;
    b = temp; 

    System.Console.WriteLine("{0} {1}", a, b);
  }
  
  public void Swap(string a, string b)
  {
    string temp = a;
    a = b;
    b = temp; 

    System.Console.WriteLine("{0} {1}", a, b);
  }

  public void Swap(Pessoa p1, Pessoa p2)
  {
    Pessoa temp = p1;
    p1 = p2;
    p2 = temp;
  }
  
  public void Swap(ref Pessoa p1, ref Pessoa p2)
  {
    Pessoa temp = p1;
    p1 = p2;
    p2 = temp;
  }

  public static void Main()
  {
    Teste v = new Teste();
   
    string x = "123", y = "456"; 
   
    System.Console.WriteLine("{0} {1}", x, y);

    v.Swap(x, y);

    System.Console.WriteLine("{0} {1}", x, y);

    System.Console.WriteLine("{0} {1}", x, y);
    
    v.Swap(ref x, ref y);
    
    System.Console.WriteLine("{0} {1}", x, y);
    
    Pessoa p1 = new Pessoa();
    p1.Nome = "Fabio"; p1.Sobrenome = "Galuppo";

    Pessoa p2 = new Pessoa();
    p2.Nome = "Wallace"; p2.Sobrenome = "Santos";

    System.Console.WriteLine("{0} {1}", p1.Nome, p1.Sobrenome);
    System.Console.WriteLine("{0} {1}", p2.Nome, p2.Sobrenome);

    v.Swap(p1, p2);
  
    System.Console.WriteLine("{0} {1}", p1.Nome, p1.Sobrenome);
    System.Console.WriteLine("{0} {1}", p2.Nome, p2.Sobrenome);     


    System.Console.WriteLine("{0} {1}", p1.Nome, p1.Sobrenome);
    System.Console.WriteLine("{0} {1}", p2.Nome, p2.Sobrenome);

    v.Swap(ref p1, ref p2);
  
    System.Console.WriteLine("{0} {1}", p1.Nome, p1.Sobrenome);
    System.Console.WriteLine("{0} {1}", p2.Nome, p2.Sobrenome);     
  } 
}
