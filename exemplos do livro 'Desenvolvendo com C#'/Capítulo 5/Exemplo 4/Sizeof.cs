//csc Sizeof.cs

using System;

struct SX{ public int x, y, z; }

class Sizeof
{    
  public static void Main()
  {      
    unsafe
    {      
      Console.WriteLine("O tamanho do byte  em bytes é {0}.", sizeof(byte));
      Console.WriteLine("O tamanho do short em bytes é {0}.", sizeof(short));
      Console.WriteLine("O tamanho do int   em bytes é {0}.", sizeof(int));
      Console.WriteLine("O tamanho do long  em bytes é {0}.", sizeof(long));
      Console.WriteLine("O tamanho do SX    em bytes é {0}.", sizeof(SX)); 
    }
  }
}
