//csc Fibonacci.cs

using System;

public class Fibonacci
{
  public static void Main()
  {	
    int iVezes;
	 		
    Console.Write("Entre de 1 a 100 para exibir uma sequência de Fibonacci:");
	
    //Verifica se os valores entrados esta entre 1 e 100
    //caso contrário pede reentrada
    do
    {	
      iVezes = Convert.ToInt32(Console.ReadLine());		
    }while(iVezes < 1 || iVezes > 100);

    //Cria o vetor dinâmicamente
    int[] iSeq = new int[iVezes];

    iSeq[0] = 1;

    //Preenche o vetor
    if(iVezes > 1)
    {
      iSeq[1] = 1;
      for(int a = 2; a < iVezes; ++a)
      iSeq[a] = iSeq[a - 1] + iSeq[a - 2];
    }

    //Exibe o vetor	
    foreach(int a in iSeq)
    {
      Console.Write(a);
      Console.Write(" ");
    }           
  }  
}
