//csc N�merosPrimos.cs /nologo

using System;
using System.Collections;

//Classe Abstrata
abstract public class ArmazenadorDeN�meros
{
  protected internal ArmazenadorDeN�meros(int capacidade)
  {
    n�meros = new ArrayList(capacidade);
  }

  abstract public void Preencher(int n�mero);  

  protected ArrayList n�meros;
}

//Classe Derivada
public class N�merosPrimos : ArmazenadorDeN�meros
{
  internal N�merosPrimos(int capacidade) : base(capacidade){}
 
  public override void Preencher(int n�mero)
  {
    bool fPrimo;
    for(int dividendo = 2; dividendo <= n�mero; ++dividendo)
    {
      fPrimo = true;
      foreach(object divisor in n�meros)
      {
        if((dividendo % (int)divisor) == 0)
        {
	  fPrimo = false;
	  break;
        }
      }
      if(fPrimo) n�meros.Add(dividendo);      
    }    
  }

  public int Come�o()
  {
    return 0;
  }
  
  public int Fim()
  {
    return n�meros.Count;
  }

  public int this[int �ndice]
  {
    get{ return (int)n�meros[�ndice]; }
  }

  public static N�merosPrimos New()
  {
    return new N�merosPrimos(100);
  } 
}

public class Aplica��o
{
  static void Main(string[] args)
  {
    DateTime �nicio = DateTime.Now;

    N�merosPrimos primos = N�merosPrimos.New(); 

    primos.Preencher(Int32.Parse(args[0]));
			
    for(int a = primos.Come�o(), l = primos.Fim(), c = 1; a < l; a++, c++)
    {
       if(c < 10)
       {
         Console.Write("{0,5} ", primos[a]);
       }
       else
       {
         Console.WriteLine("{0,5}", primos[a]);
         c = 0;
       }  
    }
   
    Console.WriteLine("\n\nTempo de processamento : {0}", DateTime.Now - �nicio);
  }
}