//csc Notificador2.cs

using System;

public delegate string Notificador(string mensagem);

public class MinhaClasse
{
  public static string NotificadorStaticHandler(string msg)
  {
    Console.WriteLine("*** {0} ***", msg);

    return "Static_S_OK";
  }

  public string NotificadorInstanceHandler(string msg)
  {
    Console.WriteLine("### {0} ###", msg);

    return "Instance_S_OK";
  }
  
  public static void Main()
  {
    MinhaClasse c = new MinhaClasse();
    
    Notificador notif = new Notificador(c.NotificadorInstanceHandler);
    notif += new Notificador(MinhaClasse.NotificadorStaticHandler);
  
    string retorno = notif("Broadcast...");

    Console.WriteLine(retorno);
  }
} 
