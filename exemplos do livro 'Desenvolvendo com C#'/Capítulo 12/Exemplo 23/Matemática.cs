//csc Matem�tica.cs

using System;

public class Matem�tica
{
  internal class Resolver
  {
    internal Resolver(System.Type T)
    {
      if(T == typeof(int) || T == typeof(double))
        t = T;
      else
        throw new InvalidCastException("Tipo n�o suportado");
    }

    internal object Value
    {
      get
      { 
        if(t == typeof(int))
          return i;
        return d;
      }
      
      set
      {
        if(t == typeof(int))
          i = (int)value;
        else 
          d = (double)value;
      }
    }

    internal void Add(object o)
    {
      if(t == typeof(int))
        i += (int)o;
      else
        try
        { 
          d += (double)o;
        }
        catch(InvalidCastException)
        {
          //somente cast int para double suportado nesta vers�o
          d += (double)(int)o;   
        }  
    }    

    private Type t;
    private int i;
    private double d; 
  }

  public object Soma(Type T, params object[] valores)
  {
    Resolver r = new Resolver(T);

    foreach(object o in valores)
    {
      r.Add(o);      
    }
    
    return r.Value;
  }

  public static void Main()
  {
    Matem�tica somador = new Matem�tica();
    Console.WriteLine(somador.Soma(typeof(int), 100, 200));
    Console.WriteLine(somador.Soma(typeof(int), 100, 200, 300));
    Console.WriteLine(somador.Soma(typeof(double), 100.1, 200.1));
    Console.WriteLine(somador.Soma(typeof(double), 100.1, 200.1, 300.1));
    Console.WriteLine(somador.Soma(typeof(double), 1d, 2d, 3d, 4d));
    Console.WriteLine(somador.Soma(typeof(double), 1, 2, 3, 4, 5));
    Console.WriteLine(somador.Soma(typeof(double), 1, 2, 3d, 4, 5, 6));
  }
} 
