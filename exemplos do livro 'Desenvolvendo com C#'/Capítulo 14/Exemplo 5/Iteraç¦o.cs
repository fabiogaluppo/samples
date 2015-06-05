//csc Iteração.cs

using System;
using System.Collections;

public class MinhaClasse : IEnumerator, IEnumerable
{
  private int[] elementos;

#region IEnumerable
  
  public IEnumerator GetEnumerator()
  {
    return this;
  }

#endregion

#region IEnumerator

  private int index = -1;

  public object Current 
  {
    get
    { 
      return elementos[index];
    }
  }

  public bool MoveNext()
  {
    if(index < elementos.Length - 1)
    {
      index++;
      return true;
    }
    return false;       
  }  
  
  public void Reset()
  {
    index = -1;
  }

#endregion

  public MinhaClasse()
  {
    Random r = new Random();
    elementos = new int[100];
    
    for(int a = 0, l = elementos.Length; a < l; ++a)
      elementos[a] = r.Next(0, Int16.MaxValue);

    Array.Sort(elementos);
  }

  public static void Main()
  {
    int b = 0;
    foreach(int a in new MinhaClasse())
    {
      Console.Write("{0,6}", a);
      if(b == 9){ b = 0; Console.WriteLine(""); } else b++; 
    }
  }
}