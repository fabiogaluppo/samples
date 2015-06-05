//csc Notificador7.cs

using System;

public delegate void Notificador(string mensagem);

public class MinhaOutraClasse
{
  public static void NotificadorStaticHandler(string msg)
  {
    Console.WriteLine("&&& {0} &&&", msg);
  }

  public void NotificadorInstanceHandler(string msg)
  {
    Console.WriteLine("%%% {0} %%%", msg);    
  }
}

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
    MinhaOutraClasse o = new MinhaOutraClasse();
    
    Notificador notif = new Notificador(MinhaClasse.NotificadorStaticHandler);
    notif += new Notificador(MinhaOutraClasse.NotificadorStaticHandler);
    notif += new Notificador(c.NotificadorInstanceHandler);
    notif += new Notificador(o.NotificadorInstanceHandler);   

    notif("Broadcast...");

    Delegate[] d = notif.GetInvocationList();

    object[] p = new object[]{"Broadcast..."};    

    d[2].DynamicInvoke(p);
    d[3].DynamicInvoke(p);
    d[1].DynamicInvoke(p);
    d[0].DynamicInvoke(p); 
  }
} 