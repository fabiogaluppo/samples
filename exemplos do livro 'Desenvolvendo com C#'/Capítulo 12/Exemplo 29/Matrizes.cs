//csc /nologo Matrizes.cs /r:Matriz.dll

using System;

public class Matrizes
{
  public static void Main()
  {
    Matriz m1 = new Matriz(2,3);
    m1[1, 1] = 7; m1[1, 2] = 6; m1[1, 3] = 8;
    m1[2, 1] = 3; m1[2, 2] = 4; m1[2, 3] = 5;
    
    Matriz m2 = new Matriz(2,3);
    m2[1, 1] = 2; m2[1, 2] = 3; m2[1, 3] = 4;
    m2[2, 1] = 5; m2[2, 2] = 6; m2[2, 3] = 7;
		
    Console.Write("Somar = M1 + M2");
   
    Matriz resultado = m1 + m2;
  
    Console.Write(resultado.ToString());

    Console.Write("\n\nSubtrair = M1 - M2");
   
    resultado = m1 - m2;      

    Console.Write(resultado.ToString());

    Console.Write("\n\nMultiplicar = M3 * M4");
   
    Matriz m3 = new Matriz(2,2);
    m3[1, 1] = 3; m3[1, 2] = 0; 
    m3[2, 1] = 1; m3[2, 2] = 1;
    
    Matriz m4 = new Matriz(2,3);
    m4[1, 1] = 3; m4[1, 2] = 2; m4[1, 3] = 8;
    m4[2, 1] = 3; m4[2, 2] = 6; m4[2, 3] = 9;
	
    resultado = m3 * m4;

    Console.Write(resultado.ToString());

    Console.Write("\n\nOposto = -M3");
  		
    resultado = -m3;

    Console.Write(resultado.ToString());

    Console.Write("\n\nMultiplicação por k = 3 * M3");
  		
    resultado = 3 * m3;

    Console.Write(resultado.ToString());

    Console.Write("\n\nExpressão = M1 + M2 - M3 * (-M4) * 2");
  		
    resultado = m1 + m2 - m3 * (-m4) * 2;

    Console.Write(resultado.ToString());

    Console.Write("\n\nExpressão = (M1 + M2 - M3 * (-M4) * 2) ^ 2");
  		
    resultado =  (m1 + m2 - m3 * (-m4) * 2) ^ 2;

    Console.Write(resultado.ToString());

    Matriz m5 = new Matriz(3,2);
    m5[1, 1] = 3; m5[1, 2] = 0; 
    m5[2, 1] = 2; m5[2, 2] = 4;
    m5[3, 1] = 5; m5[3, 2] = 7;
    
    Matriz m6 = new Matriz(2,3);
    m6[1, 1] = 3; m6[1, 2] = 2; m6[1, 3] = 5;
    m6[2, 1] = 0; m6[2, 2] = 4; m6[2, 3] = 7;

    Console.Write("\n\nM6 é matriz transposta de M5");

    Console.Write("\n{0}", m5.IsTranspost(m6) ? "Verdadeiro" : "Falso");

    Console.Write("\n\nM3 é matriz quadrada");

    Console.Write("\n{0}", m3.IsSquare() ? "Verdadeiro" : "Falso");

    int[,] mi = m3; //Conversão implícita

    Console.WriteLine("\n\nMatriz de inteiros :");

    for(int a = 0, l = mi.GetLength(0); a < l; ++a)
    {
      for(int b = 0, l2 = mi.GetLength(1); b < l2; ++b)
      {
        Console.Write("{0} ", mi[a, b]);
      }
      Console.WriteLine("");
    }

    Console.WriteLine("\nTipo de dado Matriz :");

    Matriz mt = (Matriz)mi; //Conversão explícita

    Console.Write(mt.ToString());     
  }    
}