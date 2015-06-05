//csc Notificador5.cs

using System;

public delegate string Notificador(string mensagem);

public class MinhaClasse
{
  public static string NotificadorStaticHandler(string msg)
  {
    Console.WriteLine("*** {0} ***", msg);

    return "Static_S_OK";
  }

  public static void Main()
  {
    MinhaClasse c = new MinhaClasse();
    
    Notificador notif = new Notificador(MinhaClasse.NotificadorStaticHandler);
    notif += new Notificador(MinhaClasse.NotificadorStaticHandler);
    notif += new Notificador(MinhaClasse.NotificadorStaticHandler);
  
    notif("Broadcast...");

    int totaldem�todos = notif.GetInvocationList().Length;     

    for(int a = 0; a < totaldem�todos; ++a) 
      notif -= new Notificador(MinhaClasse.NotificadorStaticHandler);    

    if(notif != null) notif("Broadcast...");    
  }
} 
