//csc Sorteio.cs

using System;

public class Sorteio
{
  public static void Main()
  { 	
    int max = 0;
			
    Console.Write("Indique um número inteiro para máximo que será sorteado:");

    try
    {	
      max = Convert.ToInt32(Console.ReadLine()); 
      Random r = new Random();    //Instância a classe Random
      int rand = r.Next(1, max);  //Sorteia randômincamente entre 0 e máximo
      Console.Write("O valor sorteado entre 1 e {1} é {0}", rand, max);	
    } 
    catch(ArgumentException)
    {
      Console.WriteLine("0 não é um valor válido"); 
    }
    catch(Exception e)
    {
      Console.WriteLine(e); 
    }  
  }
}
