//using ComparandoTipos.cs

using System;

class CX
{
  public int x;
  public int y;

  int m_z;
 
  public int z
  {
    set{ m_z = value; }
    get{ return m_z;  } 
  }  
}

struct SX
{  
  public int x;
  public int y;
} 

public class ComparandoTipos 
{
  public static void Main()
  {

    CX cx1 = new CX();
    CX cx2 = new CX();	
  
    SX sx1, sx2;   
  
    sx2.x = sx1.x = cx2.x = cx1.x = 100;
    sx2.y = sx1.y = cx2.y = cx1.y = 200;
    cx2.z = cx1.z = 300;

    //Comparando tipos primitivos
    Console.WriteLine("\nTipos Primitivos");

    if(sx1.x == sx2.x) 
      Console.WriteLine("sx1.x � igual a sx2.x"); 
    else 
      Console.WriteLine("sx1.x � diferente de sx2.x");

    if(cx1.y == cx2.y) 
      Console.WriteLine("cx1.y � igual a cx2.y"); 
    else
      Console.WriteLine("cx1.y � diferente de cx2.y");

    if(sx1.x >= sx1.y) 
      Console.WriteLine("sx1.x � maior ou igual a sx1.y"); 
    else 
      Console.WriteLine("sx1.x � menor que sx1.y");

    if(cx1.x <= cx1.y) 
      Console.WriteLine("cx1.x � menor ou igual a cx1.y"); 
    else 
      Console.WriteLine("cx1.x � maior que cx1.y");

    if(sx1.x > sx1.y)  
      Console.WriteLine("sx1.x � maior que sx1.y"); 
    else 
      Console.WriteLine("sx1.x � menor ou igual a sx1.y");
 
    if(cx1.x < cx1.y)  
      Console.WriteLine("cx1.x � menor que cx1.y"); 
    else 
      Console.WriteLine("cx1.x � maior ou igual a cx1.y");  

    if(sx1.x != sx1.y) 
      Console.WriteLine("sx1.x � diferente a sx1.y"); 
    else 
      Console.WriteLine("sx1.x � igual a sx1.y");

    //Comparando classes
    Console.WriteLine("\nClasses");

    if(cx1 == cx2) 
      Console.WriteLine("cx1 � igual a cx2"); 
    else 
      Console.WriteLine("cx1 � diferente de cx2");

    cx2 = cx1;

    if(cx1 == cx2) 
      Console.WriteLine("cx1 � igual a cx2"); 
    else 
      Console.WriteLine("cx1 � diferente de cx2");

    //Comparando structs
    Console.WriteLine("\nEstruturas");

    if((sx1.x == sx2.x) && (sx1.y == sx2.y)) 
      Console.WriteLine("sx1 � igual a sx2"); 
    else 
      Console.WriteLine("sx1 � diferente de sx2");
  }
}