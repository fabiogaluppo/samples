//csc Ponto3D.cs /unsafe

using System;
using System.Drawing; 

public struct Ponto3D
{
  private int x, y, z;  

  public Ponto3D(int x, int y, int z)
  {
    this.x = x; this.y = y; this.z = z; 
  }

  public static Ponto3D Origem()
  {   
    return new Ponto3D() ;
  }

  public override string ToString()
  {
    return string.Format("{{X={0}, Y={1}, Z={2}}}", x, y, z);
  }

  public int X{ get{ return x; } set{ x = value; } }

  public int Y{ get{ return y; } set{ y = value; } }
  
  public int Z{ get{ return z; } set{ z = value; } }

  public static Ponto3D operator+(Ponto3D lhs, Ponto3D rhs)
  {
    return new Ponto3D(lhs.x + rhs.X, lhs.y + rhs.Y, lhs.Z + rhs.Z);
  }

  public static Ponto3D operator-(Ponto3D lhs, Ponto3D rhs)
  {
    return new Ponto3D(lhs.x - rhs.X, lhs.y - rhs.Y, lhs.Z - rhs.Z);
  }

  public static explicit operator Point(Ponto3D pt)
  {
    return new Point(pt.X, pt.Y);
  }

  public static implicit operator string(Ponto3D pt)
  {
    return pt.ToString();
  }

  public static void Main()
  {
    Ponto3D p1 = new Ponto3D(10, 10, 15);
    Print(p1); //conversão implícita para string

    Ponto3D p2 = Ponto3D.Origem();
    Print(p2);

    Ponto3D p3 = p2 - p1;
    Print(p3);

    Point p = (Point)p3;
    Print(p.ToString());

    int size;

    unsafe{ size = sizeof(Ponto3D); } 

    Print(size.ToString() + " bytes");       
  }

  public static void Print(string s)
  {
    Console.WriteLine(s);
  }
}
