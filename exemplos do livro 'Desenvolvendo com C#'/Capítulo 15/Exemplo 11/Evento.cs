//csc Evento.cs

using System;

public delegate void Notificador(string mensagem);

public class CaixaDeMensagem
{
  public event Notificador NovaMensagem;

  public void EnviarMensagem(string msg)
  {
    if(NovaMensagem != null) NovaMensagem(msg);
  }
}

public class Consumidor
{
  public static void Mensagem(string msg)
  {
    Console.WriteLine("Mensagem {0} : {1}", DateTime.Now, msg);
  }
  
  public static void Main()
  {
    CaixaDeMensagem cx = new CaixaDeMensagem();

    cx.NovaMensagem += new Notificador(Consumidor.Mensagem);

    cx.EnviarMensagem("Esta � uma mensagem");
    
    System.Threading.Thread.Sleep(1000);
    
    cx.EnviarMensagem("Esta � outra mensagem");

    cx.NovaMensagem -= new Notificador(Consumidor.Mensagem);

    cx.EnviarMensagem("Esta � mensagem n�o ser� notificada");
  }
}