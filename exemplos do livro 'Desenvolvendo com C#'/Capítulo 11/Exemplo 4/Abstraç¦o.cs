//csc Abstra��o.cs

using System;

abstract public class Opera��o
{
  abstract public double Resultado(); 
}

abstract public class Opera��oUn�ria : Opera��o
{
  public double elemento;  
}

abstract public class Opera��oBin�ria : Opera��o
{
  public double elementoesquerda, elementodireita;
}

public class Oposto : Opera��oUn�ria
{
  override public double Resultado()
  {
    return -elemento;
  }
}

public class Soma : Opera��oBin�ria
{
  override public double Resultado()
  {
    return elementoesquerda + elementodireita;
  }
}

public class Subtra��o : Opera��oBin�ria
{
  override public double Resultado()
  {
    return elementoesquerda - elementodireita;
  }
}

public class M�ltiplica��o : Opera��oBin�ria
{
  override public double Resultado()
  {
    return elementoesquerda * elementodireita;
  }
}

public class Divis�o : Opera��oBin�ria
{
  override public double Resultado()
  {
    return elementoesquerda / elementodireita;
  }
}

public class Abstra��o
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

