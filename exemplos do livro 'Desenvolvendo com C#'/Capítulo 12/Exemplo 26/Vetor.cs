//csc Array.cs

using System;

public class Array
{
  public const int x = 3, y = 4, z = 5;
  
  int[,,] array; 
  
  public Array()
  {
    array = new int[Array.x, Array.y, Array.z];
    
    for(int e = 0; e < x; ++e)
      for(int f = 0; f < y; ++f)
        for(int g = 0; g < z; ++g)
          array[e, f, g] = (e + f + g) * 100; 

  }
  
  public int Length
  {
    get{ return array.Length; }
  }  

  #region Indexers
  public int this[int i]
  {
    get
    {
      unsafe
      {
        fixed(int* PArray = array)
        {
          int* parray = PArray;
          parray += i;
          return *parray;  
        }  
      }
    }
    
    set
    {
      unsafe
      {
        fixed(int* PArray = array)
        {
          int* parray = PArray;
          parray += i;
          *parray = value;  
        }  
      }
    }
  }
  
  public int this[int x, int y, int z]
  {
    get
    {
      return array[x, y, z];
    }
    
    set
    {
      lock(this){ array[x, y, z] = value; }
    }
  }
  #endregion

  public static void Main()
  {
    Array a = new Array();
    
    a[10] = 999;    
    a[9] = 777; 
    a[0, 0, 0] = 888;
    a[59] = 555;
    a[1, 2, 3] = 444; 
    
    for(int b = 0; b < a.Length; ++b)
      Console.Write("{0} ", a[b]);   
  }
}
