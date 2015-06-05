//csc Abstração.cs

using System;

abstract public class Operação
{
  abstract public double Resultado(); 
}

abstract public class OperaçãoUnária : Operação
{
  public double elemento;  
}

abstract public class OperaçãoBinária : Operação
{
  public double elementoesquerda, elementodireita;
}

public class Oposto : OperaçãoUnária
{
  override public double Resultado()
  {
    return -elemento;
  }
}

public class Soma : OperaçãoBinária
{
  override public double Resultado()
  {
    return elementoesquerda + elementodireita;
  }
}

public class Subtração : OperaçãoBinária
{
  override public double Resultado()
  {
    return elementoesquerda - elementodireita;
  }
}

public class Múltiplicação : OperaçãoBinária
{
  override public double Resultado()
  {
    return elementoesquerda * elementodireita;
  }
}

public class Divisão : OperaçãoBinária
{
  override public double Resultado()
  {
    return elementoesquerda / elementodireita;
  }
}

public class Abstração
{
  public static void Main()
  {
    Soma s = new Soma();
    s.elementoesquerda = 100D;
    s.elementodireita = 10D;
    Console.WriteLine(s.Resultado());

    Oposto o = new Oposto();
    o.elemento = 50D;
    Console.WriteLine(o.Resultado()); 
  }
}

