//csc Menu_If.cs

using System;

public class Menu_If 
{
  public static void Main()
  {	
    char chOpt;
 
    Console.WriteLine("1-Inserir");
    Console.WriteLine("2-Atualizar");
    Console.WriteLine("3-Apagar");
    Console.WriteLine("4-Procurar");
    Console.Write("Escolha entre [1] a [4]:");
	
    //Verifica se os valores entrados esta entre 1 e 4
    //caso contrário pede reentrada
    do
    {	  
      chOpt = (char)Console.Read();		
    }while(chOpt < '1' || chOpt > '4');

    if(chOpt == '1')
    {
      Console.WriteLine("Inserir...");
      //InsertFunction();
    }
    else if(chOpt == '2')
    {
      Console.WriteLine("Atualizar...");
      //UpdateFunction();
    }	 
    else if(chOpt == '3')
    {
      Console.WriteLine("Apagar...");
      //DeleteFunction();
    }
    else
    {
      Console.WriteLine("Procurar...");
      //FindFunction();  
    }
  }  
}
