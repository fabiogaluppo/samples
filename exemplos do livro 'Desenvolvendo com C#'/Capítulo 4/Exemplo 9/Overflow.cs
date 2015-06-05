//csc Overflow.cs

using System;

public class Overflow
{
  public static void Main()
  {
    try
    {
      short a = 32767;
      short b = unchecked((short)(a + 1));
	     	
      Console.WriteLine("unchecked: {1} + 1 = {0}", b, a);

      short c = 32767;
      short d = checked((short)(c + 1));
	     	
      Console.WriteLine("checked: {1} + 1 = {0}", d, c);	      
    }
    catch(OverflowException e)
    {
      Console.WriteLine("checked: Mensagem - {0}", e.Message);
    }             
  }
}
