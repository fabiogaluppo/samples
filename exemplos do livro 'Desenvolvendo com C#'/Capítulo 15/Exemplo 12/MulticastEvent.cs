//csc MulticastEvent.cs

using System;
using System.Windows.Forms;

public delegate void Notificador(string mensagem);

public class CaixaDeMensagem
{
  public event Notificador NovaMensagem;

  public void EnviarMensagem(string msg)
  {
    if(NovaMensagem != null) NovaMensagem(msg);
  }
}

public abstract class Consumidor
{
  protected const string fmt = "Mensagem {0} : {1}";  

  public abstract void Mensagem(string msg);
}

public class ConsumidorForm : Consumidor
{
  public override void Mensagem(string msg)
  {
    MessageBox.Show(String.Format(fmt, DateTime.Now, msg));
  }
}

public class ConsumidorConsole : Consumidor
{
  public override void Mensagem(string msg)
  {
    Console.WriteLine(fmt, DateTime.Now, msg);
  }
}

public class MinhaAplicação
{  
  public static void Main()
  {
    CaixaDeMensagem cx = new CaixaDeMensagem();

    ConsumidorConsole cc = new ConsumidorConsole();
    ConsumidorForm cf = new ConsumidorForm();

    cx.NovaMensagem += new Notificador(cc.Mensagem);
    cx.NovaMensagem += new Notificador(cf.Mensagem);

    cx.EnviarMensagem("Esta é uma mensagem");
    
    System.Threading.Thread.Sleep(1000);
    
    cx.EnviarMensagem("Esta é outra mensagem");

    cx.NovaMensagem -= new Notificador(cc.Mensagem);
    cx.NovaMensagem -= new Notificador(cf.Mensagem);

    cx.EnviarMensagem("Esta é mensagem não será notificada");
  }
}