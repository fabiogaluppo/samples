//csc /nologo Vetores.cs

using System;

public class Vetores
{
  public static void Main()
  {
    //Cria��o
    char[] elems = new char[6];
 
    //Atribui��o
    elems[1] = 'A';             //A
    elems[0] = (char)0x42;      //B
    elems[5] = "C# is COOL"[0]; //C
    elems[4] = 'D';             //D
    elems[3] = (char)69;        //E
    elems[2] = "EFI"[1];        //F
    
    //Percorrer
    foreach(char ch in elems) Console.WriteLine(ch);

    //Usando os membros do System.Array
    Console.WriteLine("Tamanho do array = {0}", elems.Length);
    
    Console.WriteLine("N�mero de dimens�es = {0}", elems.Rank);
    
    Console.WriteLine("�ndice do 'F' = {0}", Array.BinarySearch(elems, 'F'));
    
    int dim1 = elems.GetLength(0); 
    Console.WriteLine("N�mero de elementos na 1� dimens�o = {0}", dim1);

    Console.WriteLine("Lower Bound = {0}", elems.GetLowerBound(0));
 
    Console.WriteLine("Upper Bound = {0}", elems.GetUpperBound(0));

    Console.WriteLine("Valor na posi��o 2 = {0}", elems.GetValue(1));

    elems[5] = 'A';   

    //Primeira ocorr�ncia
    Console.WriteLine("�ndice de 'A' = {0}", Array.IndexOf(elems, 'A'));  
   
    //�ltima ocorr�ncia
    Console.WriteLine("�ndice de 'A' = {0}", Array.LastIndexOf(elems, 'A')); 

    elems.SetValue('C', 5);    

    char[] elems2 = new char[6]; 
    elems.CopyTo(elems2, 0);

    char[] elems3  = new char[6]; 
	
    Array.Sort(elems2);
    Console.WriteLine("Ordem Crescente");
    foreach(char ch in elems2) Console.Write(ch);
    Console.WriteLine("\nOrdem Decrescente");
    for(int a = elems2.Length - 1; a > -1; --a) Console.Write(elems2[a]);

    Console.WriteLine("\nOrdem Inversa do Vetor");
    Array.Reverse(elems3);
    foreach(char ch in elems3) Console.Write(ch);

    Array.Clear(elems2, 0, elems2.Length);
    Array.Clear(elems3, 0, elems3.Length);   
  }
}