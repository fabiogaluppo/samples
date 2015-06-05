//csc UserFilter.cs

using System;

public class UserFilter
{
  public static void Main()
  {
    int iDivd = 0, iDivs = 0;

    Console.Write("Entre um inteiro para o Dividendo:");	
    iDivd = Convert.ToInt32(Console.ReadLine()); 
    Console.Write("Entre um inteiro para o Divisor:");	
    iDivs = Convert.ToInt32(Console.ReadLine()); 
 
    try
    {  	
      double dResult = (double) iDivd / iDivs;
      Console.WriteLine("{0} / {1} = {2,0:N5}", iDivd, iDivs, dResult); 
    }
    catch
    {
      //Simulando o filtro  
      if(iDivd == 0 && iDivs == 0)
      {
        Console.WriteLine("0 / 0 é indeterminado");
        return;
      }
    
      if(iDivs == 0)
      {
        Console.WriteLine("0 como divisor é indefinido");	
        return;
      }
    } 
  }
} 
