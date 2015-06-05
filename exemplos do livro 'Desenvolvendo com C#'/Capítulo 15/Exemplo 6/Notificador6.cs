//csc Notificador6.cs

using System;

public delegate void Notificador(string mensagem);

public class MinhaClasse
{
  public delegate void Notificador(string mensagem);      

  public static void NotificadorStaticHandler(string msg)
  {
    Console.WriteLine("*** {0} ***", msg);
  }

  public void NotificadorInstanceHandler(string msg)
  {
    Console.WriteLine("### {0} ###", msg);    
  }

  public static void Main()
  {
    MinhaClasse c = new MinhaClasse();
    
    Notificador s_notif = new Notificador(MinhaClasse.NotificadorStaticHandler);
   
    MinhaClasse.Notificador i_notif = new MinhaClasse.Notificador(c.NotificadorInstanceHandler);

    s_notif += i_notif;

    s_notif("[1] Broadcast s_notif...");

    Notificador notif = s_notif + i_notif;
  
    notif("[2] Broadcast notif...");

    notif = s_notif - i_notif;      

    notif("[3] Broadcast notif...");

    notif = null;

    s_notif("[4] Broadcast s_notif...");

    i_notif("[5] Broadcast i_notif...");
  }
} 
