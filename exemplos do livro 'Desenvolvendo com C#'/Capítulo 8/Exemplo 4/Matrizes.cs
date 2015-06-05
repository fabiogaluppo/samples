//csc /nologo Matrizes.cs

using System;

public class Matriz
{
  private int[,] matriz;
  private int linhas, colunas;

  public Matriz(int linhas, int colunas)
  {
    this.linhas = linhas;
    this.colunas = colunas;
    matriz = new int[linhas, colunas]; 
  }
  
  public Matriz Somar(Matriz m)
  {
    Matriz res = new Matriz(this.linhas, this.colunas);

    for(int a = 0, l = linhas; a < l; ++a)
      for(int b = 0, l2 = colunas; b < l2; ++b)  
        res.SetValue(a + 1, b + 1, matriz[a, b] + m.GetValue(a + 1, b + 1));

    return res;   
  }
  
  public Matriz Subtrair(Matriz m)
  {
    Matriz res = new Matriz(this.linhas, this.colunas);

    for(int a = 0; a < linhas; ++a)
      for(int b = 0; b < colunas; ++b)  
        res.SetValue(a + 1, b + 1, matriz[a, b] - m.GetValue(a + 1, b + 1));       

    return res;
  }

  public void SetValue(int linha, int coluna, int valor)
  {
    matriz[linha - 1, coluna - 1] = valor; 
  }

  public int GetValue(int linha, int coluna)
  { 
    return matriz[linha - 1, coluna - 1]; 
  }
  
  public void Imprimir()
  {
    for(int a = 0; a < linhas; ++a)
    {
      Console.WriteLine("");
      for(int b = 0; b < colunas; ++b)
        Console.Write("{0} ", matriz[a, b]);      
    }
  }

  public static void Main()
  {
    Console.Write("\nM1");

    Matriz m1 = new Matriz(3,3);
    m1.SetValue(1, 1, 50); m1.SetValue(1, 2, 100); m1.SetValue(1, 3, 1);
    m1.SetValue(2, 1, 33); m1.SetValue(2, 2, 300); m1.SetValue(2, 3, 2);
    m1.SetValue(3, 1, 22); m1.SetValue(3, 2, 700); m1.SetValue(3, 3, 5);

    m1.Imprimir();

    Console.Write("\n\nM2");
    
    Matriz m2 = new Matriz(3,3);
    m2.SetValue(1, 1, 8); m2.SetValue(1, 2, 200); m2.SetValue(1, 3, 15);
    m2.SetValue(2, 1, 9); m2.SetValue(2, 2, 700); m2.SetValue(2, 3, 1);
    m2.SetValue(3, 1, 3); m2.SetValue(3, 2, 100); m2.SetValue(3, 3, 0);

    m2.Imprimir();
 
    Console.Write("\n\nSomar = M1 + M2");
   
    Matriz resultado = m1.Somar(m2);
  
    resultado.Imprimir();

    Console.Write("\n\nSubtrair = M1 - M2");
   
    resultado = m1.Subtrair(m2);      

    resultado.Imprimir();

    Console.WriteLine("");
  }    
}
