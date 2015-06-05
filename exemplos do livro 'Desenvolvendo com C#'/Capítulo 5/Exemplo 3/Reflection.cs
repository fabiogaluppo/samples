//csc Reflection.cs /r:System.Windows.Forms.dll

using System;
using System.Reflection;
using System.Windows.Forms;

public class Reflection
{
  public static void Main()
  {    
    Type t = typeof(MessageBox); //Obtém o tipo da classe MessageBox

    object[] op = {"Alô, Mundo!"};
    BindingFlags bindingFlags = BindingFlags.Public; 
    bindingFlags |= BindingFlags.Static;
    bindingFlags |= BindingFlags.InvokeMethod;
    t.InvokeMember("Show", bindingFlags, null, null, op);
  }
}
