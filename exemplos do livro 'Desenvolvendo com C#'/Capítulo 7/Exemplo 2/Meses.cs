//csc Meses.cs

using System;

class Meses
{  
  enum Mês
  {
    Janeiro = 1, 
    Fevereiro, 
    Março, 
    Abril, 
    Maio, 
    Junho, 
    Julho,
    Agosto,
    Setembro,
    Outubro,
    Novembro,
    Dezembro
  }

  public static void Main()
  {    
    Mês mes = (Mês)Convert.ToByte(DateTime.Today.Month);
    Console.WriteLine("O mês é {0}", mes);
     
    mes = Mês.Dezembro;
    //if(mes == 12) //errado
    if(mes == (Mês) 12) Console.WriteLine("Dezembro");
    
    //mes = 11 //errado
    mes = (Mês)11;

    //int m = mes //errado
    int m = (int)mes;     
  }
}
