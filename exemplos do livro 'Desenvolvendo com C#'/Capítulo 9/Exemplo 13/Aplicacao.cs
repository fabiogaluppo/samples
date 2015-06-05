//csc Aplicacao.cs /r:System.Windows.Forms.dll

using System;
using System.Windows.Forms;

internal class MyOwnException : Exception
{
  public MyOwnException() : base(){}  
  public MyOwnException(string msg) : base(msg){}
  public MyOwnException(string msg, Exception inner) : base(msg, inner){}
}

public class Aplicacao
{
  public static void Main()
  {
    try
    {
      throw new MyOwnException("Exceção disparada...", new SystemException());
    }
    catch(MyOwnException e)
    {
      e.Source = "classe Aplicacao";
      MessageBoxButtons but = MessageBoxButtons.OK;
      MessageBoxIcon ico = MessageBoxIcon.Error;
      MessageBox.Show(e.Message, e.Source, but, ico);
    }
  }
}
