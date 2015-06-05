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
    
    public string Obter(int �ndice)
    {
      if(�ndice < 1) 
        throw new IndexOutOfRangeException("O �ndice deve ser maior que zero");
      return m_Array[�ndice - 1].ToString();
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

  public MinhaClasse.MinhaClasseAninhada ObterInst�ncia(int capacidade)
  {
    return new MinhaClasse.MinhaClasseAninhada(capacidade);
  }

  public static void Main()
  {
    MinhaClasse c = new MinhaClasse();
    
    MinhaClasse.MinhaClasseAninhada ac = c.ObterInst�ncia(5);
    
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
