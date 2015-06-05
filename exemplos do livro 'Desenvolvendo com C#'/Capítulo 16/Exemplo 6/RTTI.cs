//csc RTTI.cs /r:DeveloperAttribute.dll /r:MessageQueue.dll

using System;
using System.Reflection;

public class RTTI
{
  public static void Main()
  {
    Type t = typeof(MessageQueue);    

    Dump(t);
 
    foreach(MemberInfo mi in t.GetMembers())
      Dump(mi);   
  }

  public static void Dump(MemberInfo mi)
  {
    object[] attrs = (object[])mi.GetCustomAttributes(true);  
   
    foreach(object o in attrs)
    {
      DeveloperAttribute d = o as DeveloperAttribute;
      if(d != null)
        Console.WriteLine("{2,12}\t{1,15}\t {0}", d.Name, d.CodeStatus, mi.Name);
    }
  }
}
