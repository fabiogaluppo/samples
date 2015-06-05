//csc Interface.cs

public delegate void Notifica��o();

public interface Interface
{
  void M�todo();

  int Propriedade{ set; }

  int this[int a]{ get; }
  
  event Notifica��o EventoNotifica��o;
}

public class Classe : Interface
{
  int valor;  

  public void EventoNotifica��oHandler(){ System.Console.Write("Disparou!!!"); } 
 
  public void M�todo(){ EventoNotifica��o(); }

  public int Propriedade{ set{ valor = value;} }

  public int this[int a]{ get{ return valor;} }

  public event Notifica��o EventoNotifica��o;

  public static void Main()
  {
    Classe c = new Classe();
    
    c.EventoNotifica��o += new Notifica��o(c.EventoNotifica��oHandler);
    
    c.M�todo(); 
  }
}