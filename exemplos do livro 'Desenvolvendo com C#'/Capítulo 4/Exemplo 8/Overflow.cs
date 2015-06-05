//csc Overflow.cs /checked+
//csc Overflow.cs /checked-

using System;

public class Overflow
{
  public static void Main()
  {
    try
    {
      short a = 32767;
      short b = (short)(a + 1);
	     	
      Console.Write("{1} + 1 = {0}", b, a);          
    }
    catch(OverflowException e)
    {
       Console.WriteLine("Mensagem: {0}", e.Message);
    }        
  }
}
