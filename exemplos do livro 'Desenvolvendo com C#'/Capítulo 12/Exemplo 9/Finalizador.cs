//csc Finalizador.cs

using System;
using System.Runtime.InteropServices;

public class Derivada : Base
{
  public Derivada()
  {
    Console.WriteLine("Consome recurso - Derivada");
  }

  ~Derivada() //finalizador ou destrutor não determinístico
  {
    Console.WriteLine("Libera recurso - Derivada");   
  }
}

public class Base
{
  private GCHandle handle;

  public Base() //construtor
  {
    Console.WriteLine("Consome recurso - Base");

    handle = GCHandle.Alloc(this);
  }

  public int Pointer{ get { return ((IntPtr)handle).ToInt32(); } }  

  ~Base() //finalizador ou destrutor não determinístico
  {
    Console.WriteLine("Libera recurso - Base");
    
    handle.Free();
  }  

  public static void Main()
  {
    Base b = new Base(); 
    Derivada d = new Derivada();    

    Console.WriteLine("{0:X}", b.Pointer);
    Console.WriteLine("{0:X}", d.Pointer);
    
    //~Finalizador() chamado na coleta
  }
}
