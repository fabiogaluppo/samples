//csc Meses.cs

using System;

class Meses
{  
  enum M�s
  {
    Janeiro = 1, 
    Fevereiro, 
    Mar�o, 
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
    M�s mes = (M�s)Convert.ToByte(DateTime.Today.Month);
    Console.WriteLine("O m�s � {0}", mes);
     
    mes = M�s.Dezembro;
    //if(mes == 12) //errado
    if(mes == (M�s) 12) Console.WriteLine("Dezembro");
    
    //mes = 11 //errado
    mes = (M�s)11;

    //int m = mes //errado
    int m = (int)mes;     
  }
}
