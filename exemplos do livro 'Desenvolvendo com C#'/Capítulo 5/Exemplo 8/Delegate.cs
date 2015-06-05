//csc Delegate.cs /r:System.Windows.Forms.dll 

using System;
using System.Windows.Forms;

public class Delegate
{
  delegate void Trace(string s);

  public static void Main()
  {
    Trace trace = new Trace(TraceConsole);   
    Trace trace2 = new Trace(TraceMsgBox); 	
    trace += trace2;
    trace("Isto é um delegate com 2 notificações!");
    trace -= trace2;
    trace("Isto é um delegate com 1 notificação!");
  }

  static void TraceConsole(string st)
  {
    Console.WriteLine(st);
  }

  static void TraceMsgBox(string st)
  {
    MessageBox.Show(st);
  }
}
