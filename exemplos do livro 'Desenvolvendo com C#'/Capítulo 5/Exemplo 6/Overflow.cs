//csc Overflow.cs /checked+
//csc Overflow.cs /checked-

using System; 

public class Overflow
{
  public static void Main()
  {
    byte x = 255; 
    System.Console.Write(++x);
  }
}
