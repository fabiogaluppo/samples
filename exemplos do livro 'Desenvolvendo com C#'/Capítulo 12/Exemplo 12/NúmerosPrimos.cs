//csc NúmerosPrimos.cs /nologo

using System;
using System.Collections;

//Classe Abstrata
abstract public class ArmazenadorDeNúmeros
{
  protected internal ArmazenadorDeNúmeros(int capacidade)
  {
    números = new ArrayList(capacidade);
  }

  abstract public void Preencher(int número);  

  protected ArrayList números;
}

//Classe Derivada
public class NúmerosPrimos : ArmazenadorDeNúmeros
{
  internal NúmerosPrimos(int capacidade) : base(capacidade){}
 
  public override void Preencher(int número)
  {
    bool fPrimo;
    for(int dividendo = 2; dividendo <= número; ++dividendo)
    {
      fPrimo = true;
      foreach(object divisor in números)
      {
        if((dividendo % (int)divisor) == 0)
        {
	  fPrimo = false;
	  break;
        }
      }
      if(fPrimo) números.Add(dividendo);      
    }    
  }

  public int Começo()
  {
    return 0;
  }
  
  public int Fim()
  {
    return números.Count;
  }

  public int this[int índice]
  {
    get{ return (int)números[índice]; }
  }

  public static NúmerosPrimos New()
  {
    return new NúmerosPrimos(100);
  } 
}

public class Aplicação
{
  static void Main(string[] args)
  {
    DateTime ínicio = DateTime.Now;

    NúmerosPrimos primos = NúmerosPrimos.New(); 

    primos.Preencher(Int32.Parse(args[0]));
			
    for(int a = primos.Começo(), l = primos.Fim(), c = 1; a < l; a++, c++)
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
   
    Console.WriteLine("\n\nTempo de processamento : {0}", DateTime.Now - ínicio);
  }
}