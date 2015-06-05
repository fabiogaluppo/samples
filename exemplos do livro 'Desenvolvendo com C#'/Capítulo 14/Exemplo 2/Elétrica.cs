//csc Elétrica.cs

using System;

public struct Corrente
{
  private double i;  

  public override string ToString()
  {
    return string.Format("{0} A", i);
  }

  public double Valor{ get{ return i; } set{ i = value; } }

  public static Corrente operator+(Corrente lhs, Corrente rhs)
  {
    lhs.Valor += rhs.Valor; return lhs;
  }

  public static Corrente operator-(Corrente lhs, Corrente rhs)
  {
    lhs.Valor -= rhs.Valor; return lhs;
  }

  public static implicit operator Corrente(int value)
  {
    Corrente crt = (double)value;

    return crt;
  }  

  public static implicit operator Corrente(double value)
  {
    Corrente crt = new Corrente();

    crt.Valor = value;
 
    return crt;
  }

  public static Corrente Calcular(Potência p, Tensão t)
  {
    Corrente crt = new Corrente();

    crt.Valor = p.Valor / t.Valor;
    
    return crt;   
  }
}

public struct Tensão
{
  private double u;  

  public override string ToString()
  {
    return string.Format("{0} V", u);
  }

  public double Valor{ get{ return u; } set{ u = value; } }

  public static Tensão operator+(Tensão lhs, Tensão rhs)
  {
    lhs.Valor += rhs.Valor; return lhs;
  }

  public static Tensão Calcular(Tensão lhs, Tensão rhs)
  {
    lhs.Valor -= rhs.Valor; return lhs;
  }

  public static implicit operator Tensão(int value)
  {
    Tensão tns = (double)value;

    return tns;
  }

  public static implicit operator Tensão(double value)
  {
    Tensão tns = new Tensão();

    tns.Valor = value;
 
    return tns;
  }

  public static Tensão Calcular(Potência p, Corrente c)
  {
    Tensão tns = new Tensão();
    
    tns.Valor = p.Valor / c.Valor;
    
    return tns;   
  }
}

public struct Potência
{
  private double p;  

  public override string ToString()
  {
    return string.Format("{0} W", p);
  }

  public double Valor{ get{ return p; } set{ p = value; } }

  public static Potência operator+(Potência lhs, Potência rhs)
  {
    lhs.Valor += rhs.Valor; return lhs;
  }

  public static Potência operator-(Potência lhs, Potência rhs)
  {
    lhs.Valor -= rhs.Valor; return lhs;
  }

  public static implicit operator Potência(int value)
  {
    Potência pot = (double)value;

    return pot;
  }

  public static implicit operator Potência(double value)
  {
    Potência pot = new Potência();

    pot.Valor = value;
 
    return pot;
  }

  public static Potência Calcular(Tensão t, Corrente c)
  {
    return Potência.Calcular(c, t);  
  }

  public static Potência Calcular(Corrente c, Tensão t)
  {
    Potência pot = new Potência();
    
    pot.Valor = c.Valor * t.Valor;
    
    return pot;   
  }
}

public struct Aplicação
{
  public static void Main()
  {
    Tensão t1 = 220;    
    Corrente c1 = 2;
    
    Potência p1 = Potência.Calcular(t1, c1);
    Print(p1);
    
    p1 = 1500;
    t1 = 110;
    Print(Corrente.Calcular(p1, t1));

    c1 = 1.5;
    Print(Tensão.Calcular(p1, c1));
  }

  public static void Print(object o)
  {
    Console.WriteLine("{0} = {1}", o.GetType().FullName, o.ToString());
  }
}
