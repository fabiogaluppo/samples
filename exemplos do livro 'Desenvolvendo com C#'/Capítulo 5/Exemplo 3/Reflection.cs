//csc Reflection.cs /r:System.Windows.Forms.dll

using System;
using System.Reflection;
using System.Windows.Forms;

public class Reflection
{
  public static void Main()
  {    
    Type t = typeof(MessageBox); //Obt�m o tipo da classe MessageBox

    object[] op = {"Al�, Mundo!"};
    BindingFlags bindingFlags = BindingFlags.Public; 
    bindingFlags |= BindingFlags.Static;
    bindingFlags |= BindingFlags.InvokeMethod;
    t.InvokeMember("Show", bindingFlags, null, null, op);
  }
}
