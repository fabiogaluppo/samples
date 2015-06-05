//csc Copiar.cs /r:CopiarGUI.dll /lib:.\GUI /win32icon:Copy.ico

using System;
using System.IO;
using System.Threading;

public class Copiar
{
  private delegate void CopiarArquivoDelegate(string d, string s, Formulário f);  

  public void CopiarCallback(IAsyncResult iar)
  {
    CopiarArquivoDelegate af = (CopiarArquivoDelegate)iar.AsyncState;
    
    af.EndInvoke(iar);
  }

  public void AsyncExec(object state)
  {
    string[] args = ((string)state).Split(';');

    int tNum = Thread.CurrentThread.GetHashCode();
    Console.WriteLine("AsyncExec {0} - Thread {1}", args[1], tNum);
    
    Copiar cp = new Copiar();

    CopiarArquivoDelegate cd = new CopiarArquivoDelegate(cp.CopiarArquivo);

    AsyncCallback cb = new AsyncCallback(cp.CopiarCallback);

    Formulário form = new Formulário();    

    form.Text = args[1];
    
    IAsyncResult iar = cd.BeginInvoke(args[1], args[0], form, cb, cd);

    form.ShowDialog();

    if(form.IsCanceled && VerificarSeArquivoExiste(form.Text))
    { 
      iar.AsyncWaitHandle.WaitOne();

      File.Delete(form.Text); 
    }
  }

  public static void Main(string[] args)
  {
    if(args.Length < 2 || args.Length % 2 != 0)
    {
      ModoUsar();
      return;      
    }

    for(int a = 0; a < args.Length; a += 2)
    {    
      if(!VerificarSeArquivoExiste(args[a]))
      {
        Console.WriteLine("{0} não existe", args[a]);
        continue;
      }

      WaitCallback cb = new WaitCallback(new Copiar().AsyncExec);

      ThreadPool.QueueUserWorkItem(cb, args[a] + ";" + args[a + 1]);
    }

    Console.WriteLine("Main Thread {0}", Thread.CurrentThread.GetHashCode());  

    Console.ReadLine();
  }
 
  private static void ModoUsar()
  {
    Console.WriteLine("Modo de usar:\nCopiar {0} [{0}]...", "origem destino");
  }

  private static bool VerificarSeArquivoExiste(string nomeArquivo)
  {
    return File.Exists(nomeArquivo);
  }

  private void CopiarArquivo(string destino, string origem, Formulário f)
  {
    int tNum = Thread.CurrentThread.GetHashCode();
    Console.WriteLine("CopiarArquivo {0} - Thread {1}", destino, tNum);
    
    if(f.IsCanceled) return;
    
    FileStream fsOrigem, fsDestino;

    using(fsOrigem = new FileStream(origem, FileMode.Open, FileAccess.Read))
    {
      using(fsDestino = new FileStream(destino, FileMode.Create))
      {
        try
        { 
          int maxsize = (int)fsOrigem.Length;
          int bufsize =  (int)(maxsize * 0.1);
          f.progresso.Step = 1;
          f.progresso.Maximum = 11; 

          byte[] buffer = new byte[bufsize];
          for(int offset = 0; offset < maxsize; offset += bufsize)
          {
            f.progresso.Value++;
            
            if(f.IsCanceled) break;
            
            fsOrigem.Read(buffer, 0, bufsize);
            
            if(f.IsCanceled) break;
            
            fsDestino.Write(buffer, 0, bufsize);
          }
        }
        catch(Exception e)
        {
          Console.WriteLine("Mensagem de Erro : {0}", e);          
        }        
      }      
    }
    
    f.Close();
  }
}
