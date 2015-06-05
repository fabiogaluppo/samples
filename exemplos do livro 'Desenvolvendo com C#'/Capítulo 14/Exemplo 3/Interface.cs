//csc Interface.cs

public delegate void Notificação();

public interface Interface
{
  void Método();

  int Propriedade{ set; }

  int this[int a]{ get; }
  
  event Notificação EventoNotificação;
}

public class Classe : Interface
{
  int valor;  

  public void EventoNotificaçãoHandler(){ System.Console.Write("Disparou!!!"); } 
 
  public void Método(){ EventoNotificação(); }

  public int Propriedade{ set{ valor = value;} }

  public int this[int a]{ get{ return valor;} }

  public event Notificação EventoNotificação;

  public static void Main()
  {
    Classe c = new Classe();
    
    c.EventoNotificação += new Notificação(c.EventoNotificaçãoHandler);
    
    c.Método(); 
  }
}