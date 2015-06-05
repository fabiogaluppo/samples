//csc NumerosRomanos.cs

using System;

public enum NumerosRomanos : byte
{
  I = 1, 
  II,
  III,
  IV,
  V,
  VI,
  VII,
  VIII,
  IX,
  X
}

public class Numeros
{
  public static void Main()
  {
    NumerosRomanos num = NumerosRomanos.I;
    Console.WriteLine("{1} = {0}", (byte)num, num); //{0} = (byte)num; {1} = num 
    
    num = NumerosRomanos.X;
    Console.WriteLine("{1} = {0}\n", (byte)num, num);

    if(num == NumerosRomanos.X)
    {
      string txt = "Números Romanos";
      Console.WriteLine(txt);
      for(int a = 0; a < txt.Length; ++a)
        Console.Write("-");

      Console.WriteLine("");  
      
      foreach(string _num in System.Enum.GetNames(typeof(NumerosRomanos))) 
        Console.Write("{0} ", _num);
    }

  }
}
