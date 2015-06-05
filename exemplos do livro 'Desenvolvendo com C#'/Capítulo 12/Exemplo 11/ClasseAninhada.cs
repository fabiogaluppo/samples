//csc ClasseAninhada.cs

using System;
using System.Collections;

public class MinhaClasse
{ 
  #region MinhaClasseAninhada
  
  public class MinhaClasseAninhada
  {
    private ArrayList m_Array;
   
    internal MinhaClasseAninhada(int capacidade)
    {
      m_Array = new ArrayList(capacidade);
    }
    
    public void Inserir(string valor)
    {
      m_Array.Add(valor);
    }
    
    public string Obter(int índice)
    {
      if(índice < 1) 
        throw new IndexOutOfRangeException("O índice deve ser maior que zero");
      return m_Array[índice - 1].ToString();
    }
    
    public void Remover(string valor)
    {   
      m_Array.Remove(valor);
    }
    
    protected internal int TotalDeItens
    {
      get{ return m_Array.Count; }
    }
  }
  
  #endregion

  public MinhaClasse.MinhaClasseAninhada ObterInstância(int capacidade)
  {
    return new MinhaClasse.MinhaClasseAninhada(capacidade);
  }

  public static void Main()
  {
    MinhaClasse c = new MinhaClasse();
    
    MinhaClasse.MinhaClasseAninhada ac = c.ObterInstância(5);
    
    ac.Inserir("Verde");
    ac.Inserir("Branco");
    ac.Inserir("Amarelo");
    ac.Inserir("Azul");
    ac.Inserir("Preto");
    
    Console.WriteLine(ac.TotalDeItens); //5
    
    ac.Remover("Preto");
   
    Console.WriteLine(ac.TotalDeItens); //4
    
    string item = ac.Obter(2); //Branco

    Console.WriteLine(item);
  }
}
