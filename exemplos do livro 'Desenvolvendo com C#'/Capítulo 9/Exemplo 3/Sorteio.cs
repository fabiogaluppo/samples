//csc Sorteio.cs

using System;

public class Sorteio
{
  public static void Main()
  { 	
    int max = 0;
			
    Console.Write("Indique um n�mero inteiro para m�ximo que ser� sorteado:");

    try
    {	
      max = Convert.ToInt32(Console.ReadLine()); 
      Random r = new Random();    //Inst�ncia a classe Random
      int rand = r.Next(1, max);  //Sorteia rand�mincamente entre 0 e m�ximo
      Console.Write("O valor sorteado entre 1 e {1} � {0}", rand, max);	
    } 
    catch(ArgumentException)
    {
      Console.WriteLine("0 n�o � um valor v�lido"); 
    }
    catch(Exception e)
    {
      Console.WriteLine(e); 
    }  
  }
}
