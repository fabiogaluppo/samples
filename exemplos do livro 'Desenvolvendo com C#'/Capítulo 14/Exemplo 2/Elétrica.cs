//csc El�trica.cs

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

  public static Corrente Calcular(Pot�ncia p, Tens�o t)
  {
    Corrente crt = new Corrente();

    crt.Valor = p.Valor / t.Valor;
    
    return crt;   
  }
}

public struct Tens�o
{
  private double u;  

  public override string ToString()
  {
    return string.Format("{0} V", u);
  }

  public double Valor{ get{ return u; } set{ u = value; } }

  public static Tens�o operator+(Tens�o lhs, Tens�o rhs)
  {
    lhs.Valor += rhs.Valor; return lhs;
  }

  public static Tens�o Calcular(Tens�o lhs, Tens�o rhs)
  {
    lhs.Valor -= rhs.Valor; return lhs;
  }

  public static implicit operator Tens�o(int value)
  {
    Tens�o tns = (double)value;

    return tns;
  }

  public static implicit operator Tens�o(double value)
  {
    Tens�o tns = new Tens�o();

    tns.Valor = value;
 
    return tns;
  }

  public static Tens�o Calcular(Pot�ncia p, Corrente c)
  {
    Tens�o tns = new Tens�o();
    
    tns.Valor = p.Valor / c.Valor;
    
    return tns;   
  }
}

public struct Pot�ncia
{
  private double p;  

  public override string ToString()
  {
    return string.Format("{0} W", p);
  }

  public double Valor{ get{ return p; } set{ p = value; } }

  public static Pot�ncia operator+(Pot�ncia lhs, Pot�ncia rhs)
  {
    lhs.Valor += rhs.Valor; return lhs;
  }

  public static Pot�ncia operator-(Pot�ncia lhs, Pot�ncia rhs)
  {
    lhs.Valor -= rhs.Valor; return lhs;
  }

  public static implicit operator Pot�ncia(int value)
  {
    Pot�ncia pot = (double)value;

    return pot;
  }

  public static implicit operator Pot�ncia(double value)
  {
    Pot�ncia pot = new Pot�ncia();

    pot.Valor = value;
 
    return pot;
  }

  public static Pot�ncia Calcular(Tens�o t, Corrente c)
  {
    return Pot�ncia.Calcular(c, t);  
  }

  public static Pot�ncia Calcular(Corrente c, Tens�o t)
  {
    Pot�ncia pot = new Pot�ncia();
    
    pot.Valor = c.Valor * t.Valor;
    
    return pot;   
  }
}

public struct Aplica��o
{
  public static void Main()
  {
    Tens�o t1 = 220;    
    Corrente c1 = 2;
    
    Pot�ncia p1 = Pot�ncia.Calcular(t1, c1);
    Print(p1);
    
    p1 = 1500;
    t1 = 110;
    Print(Corrente.Calcular(p1, t1));

    c1 = 1.5;
    Print(Tens�o.Calcular(p1, c1));
  }

  public static void Print(object o)
  {
    Console.WriteLine("{0} = {1}", o.GetType().FullName, o.ToString());
  }
}
