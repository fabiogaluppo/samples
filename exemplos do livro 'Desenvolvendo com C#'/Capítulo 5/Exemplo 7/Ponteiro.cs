//csc Ponteiro.cs /unsafe

using System;

struct SX{ public int x; } 

public class Ponteiro
{

  public static void Main()
  {

    SX sx; 
    sx.x = 0;

    unsafe
    {
      SX* psx = &sx;  
      psx->x = 10;  
    } 	  

    System.Console.Write(sx.x);
  }
}
