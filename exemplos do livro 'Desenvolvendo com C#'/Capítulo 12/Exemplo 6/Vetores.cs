//csc Vetores.cs

using System;

public class Vetores
{
  public void PreencherVal(byte[] arr)
  {
    for(byte a = byte.MinValue, l = byte.MaxValue; a < l; ++a)
      arr[a] = a; 
  }

  public void PreencherRef(ref byte[] arr)
  {
    for(byte a = byte.MinValue, l = byte.MaxValue; a < l; ++a)
      arr[a] = a; 
  }

  public void PreencherOut(out byte[] arr)
  {
    arr = new byte[255];

    for(byte a = byte.MinValue, l = byte.MaxValue; a < l; ++a)
      arr[a] = a;
  }

  public static void Main()
  {
    Vetores v = new Vetores();
   
    byte[] _val = new byte[255];
    for(byte a = byte.MinValue, l = byte.MaxValue; a < l; ++a) _val[a] = 1;
 
    v.PreencherVal(_val);

    Console.WriteLine("ByVal");
    for(byte a = byte.MinValue, l = byte.MaxValue; a < l; ++a)   
      Console.Write("{0,3} ", _val[a]);

    Console.WriteLine();
    
    byte[] _ref = new byte[255];
    for(byte a = byte.MinValue, l = byte.MaxValue; a < l; ++a) _ref[a] = 1;
 
    v.PreencherRef(ref _ref);    

    Console.WriteLine("ByRef");
    for(byte a = byte.MinValue, l = byte.MaxValue; a < l; ++a)   
      Console.Write("{0,3} ", _ref[a]);

    Console.WriteLine();
    
    byte[] _out;
    
    v.PreencherOut(out _out);    

    Console.WriteLine("ByRef - out");
    for(byte a = byte.MinValue, l = byte.MaxValue; a < l; ++a)   
      Console.Write("{0,3} ", _out[a]);
  }
}
