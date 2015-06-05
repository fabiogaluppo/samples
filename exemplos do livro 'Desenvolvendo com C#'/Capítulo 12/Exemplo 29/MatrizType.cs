//csc /nologo /target:library /out:Matriz.dll MatrizType.cs

using System;

internal abstract class AbstractMatrizIterator
{
  protected int linhas, colunas;

  protected int linha, coluna;
 
  protected bool fim;

  public virtual int Linha{ get{ return linha + 1; } }
  
  public virtual int Coluna{ get{ return coluna + 1; } }

  public virtual int Linhas{ get{ return linhas; } }
  
  public virtual int Colunas{ get{ return colunas; } }      

  public virtual void next()
  {
    if(fim) return;

    if(++coluna == colunas)
    {
      coluna = 0;
      if(++linha == linhas){ coluna--; linha--; fim = true; }
    }    
  }

  public virtual void begin()
  {
    linha = coluna = 0;

    fim = false;
  }  

  public virtual bool end()
  {
    return fim;
  }
}

internal class MatrizMulIterator : AbstractMatrizIterator
{
  private int linhaL, linhaR, colunaL, colunaR;
  private int linhasR, colunasL;

  private bool innerfim;
  
  public int LinhaL{ get{ return linhaL; } }

  public int LinhaR{ get{ return linhaR; } }

  public int ColunaL{ get{ return colunaL; } }

  public int ColunaR{ get{ return colunaR; } }    

  public MatrizMulIterator(Matriz ml, Matriz mr)
  {
    linhas  = ml.Linhas;
    colunas = mr.Colunas;

    linhasR = mr.Linhas;
    colunasL = ml.Colunas;

    begin();
    innerbegin();
  }
	
  public void innernext()
  {
    if(innerfim) return;

    if(++colunaL > linhasR || ++linhaR > colunasL)
    { 
      colunaL--; linhaR--; innerfim = true; 
    }
  }

  public void innerbegin()
  {
    linhaL = linha + 1;
    colunaL = linhaR = 1;
    colunaR = coluna + 1;     

    innerfim = false;
  }  

  public bool innerend()
  {
    return innerfim;
  }  
}

internal class MatrizIterator : AbstractMatrizIterator
{
  public MatrizIterator(Matriz m)
  {
    linhas  = m.Linhas;
    colunas = m.Colunas;		

    begin();
  }
  
  public MatrizIterator(Matriz ml, Matriz mr)
  {
    if(ml.Linhas < mr.Linhas || ml.Colunas < mr.Colunas)
      throw new IndexOutOfRangeException("Matrizes sem dimensões iguais");

    linhas  = ml.Linhas;
    colunas = ml.Colunas;
	  
    begin();
  }
}

public struct Matriz
{
  private int[,] matriz;
  private int linhas, colunas;

  public int Linhas{ get{ return linhas; } }  

  public int Colunas{ get{ return colunas; } }
  
  private void CheckBounds(int linha, int coluna)
  {
    if(linha < 0 || linha > linhas) 
      throw new IndexOutOfRangeException("Linha fora dos limites");

    if(coluna < 0 || coluna > colunas)  
      throw new IndexOutOfRangeException("Coluna fora dos limites");
  }

  public int this[int linha, int coluna]
  {
    get
    {
      CheckBounds(--linha, --coluna);

      return matriz[linha, coluna];
    }

    set
    {
      CheckBounds(--linha, --coluna);

      matriz[linha, coluna] = value;
    }
  }  

  public Matriz(int linhas, int colunas)
  {
    this.linhas = linhas;
    this.colunas = colunas;
    matriz = new int[linhas, colunas]; 
  }  
  
  public bool IsTranspost(Matriz m)
  {
    if(this.Linhas != m.Colunas || this.Colunas != m.Linhas)
      return false;

    MatrizIterator mi = new MatrizIterator(this);
    int lin, col;
    for(mi.begin(); !mi.end(); mi.next())
    {       
      lin = mi.Linha; col = mi.Coluna;
      if(this[lin, col] != m[col, lin]) return false;
    }

    return true;
  }

  public bool IsNull()
  {
    MatrizIterator mi = new MatrizIterator(this);
    
    for(mi.begin(); !mi.end(); mi.next())
    {      
      if(this[mi.Linha, mi.Coluna] != 0) return false;
    }

    return true;
  }

  public bool IsSquare()
  {
    return linhas == colunas;
  }

  public static Matriz operator+(Matriz ml, Matriz mr)
  {
    MatrizIterator mi = new MatrizIterator(ml, mr);
    Matriz res = new Matriz(mi.Linhas, mi.Colunas);
    
    int lin, col;
    for(mi.begin(); !mi.end(); mi.next())
    {       
      lin = mi.Linha; col = mi.Coluna;
      res[lin, col] = ml[lin, col] + mr[lin, col];
    }
    
    return res;      
  }

  public static Matriz operator-(Matriz ml, Matriz mr)
  {
    MatrizIterator mi = new MatrizIterator(ml, mr);
    Matriz res = new Matriz(mi.Linhas, mi.Colunas);
    
    int lin, col;
    for(mi.begin(); !mi.end(); mi.next())
    {       
      lin = mi.Linha; col = mi.Coluna;
      res[lin, col] = ml[lin, col] - mr[lin, col];
    }
    
    return res;   
  }

  public static Matriz operator-(Matriz m)
  {
    MatrizIterator mi = new MatrizIterator(m);
    Matriz res = new Matriz(m.Linhas, m.Colunas);
    
    int lin, col;
    for(mi.begin(); !mi.end(); mi.next())
    {       
      lin = mi.Linha; col = mi.Coluna;
      res[lin, col] = -m[lin, col];
    }
    
    return res;   
  }

  public static Matriz operator*(Matriz m, int k)
  {
    MatrizIterator mi = new MatrizIterator(m);
    Matriz res = new Matriz(mi.Linhas, mi.Colunas);
    
    int lin, col;
    for(mi.begin(); !mi.end(); mi.next())
    {       
      lin = mi.Linha; col = mi.Coluna;
      res[lin, col] = m[lin, col] * k;
    }
    
    return res;   
  }

  public static Matriz operator*(int k, Matriz m)
  {
    return m * k;
  }

  public static bool operator==(Matriz ml, Matriz mr)
  {
    return ml.Equals(mr);
  }

  public static bool operator!=(Matriz ml, Matriz mr)
  {
    return !ml.Equals(mr);
  }

  public static Matriz operator^(Matriz m, uint k)
  {
    Matriz res = (Matriz)m.MemberwiseClone();

    for(uint a = 1; a < k; ++a)
    {
      res *= m;
    }

    return res;
  }

  public static Matriz operator*(Matriz ml, Matriz mr)
  {
    MatrizMulIterator mmi = new MatrizMulIterator(ml, mr);
    Matriz res = new Matriz(mmi.Linhas, mmi.Colunas);
    
    int lin, col;
    for(mmi.begin(); !mmi.end(); mmi.next())
    {       
      lin = mmi.Linha; col = mmi.Coluna;
      for(mmi.innerbegin(); !mmi.innerend(); mmi.innernext())
      {        
        res[lin, col] += ml[mmi.LinhaL, mmi.ColunaL] * mr[mmi.LinhaR, mmi.ColunaR];
      }
    }
    
    return res;      
  }

  public override int GetHashCode()
  {
    return this.GetHashCode();
  }

  public override bool Equals(object m)
  {
    Matriz mm = (Matriz)m;

    if(this.Colunas != mm.Colunas || this.Linhas != mm.Linhas)
      return false;

    MatrizIterator mi = new MatrizIterator(this);
		
    int lin, col;
    for(mi.begin(); !mi.end(); mi.next())
    {       
        lin = mi.Linha; col = mi.Coluna;
        if(this[lin, col] != mm[lin, col])
          return false;
    }

    return true;
  }
	
  public override string ToString()
  {
    System.Text.StringBuilder sb = new System.Text.StringBuilder(50); 
		
    int lastLine = 0;

    MatrizIterator mi = new MatrizIterator(this);

    for(mi.begin(); !mi.end(); mi.next())
    {       
      if(lastLine != mi.Linha){ lastLine = mi.Linha; sb.Append("\n"); }
 
      sb.AppendFormat("{0,6} ", this[mi.Linha, mi.Coluna]);
    }

    return sb.ToString();
  }

  public static implicit operator int[,](Matriz m)
  {
    int[,] mint = new int[m.Linhas, m.Colunas];
    
    MatrizIterator mi = new MatrizIterator(m);

    for(mi.begin(); !mi.end(); mi.next())
    {       
      mint[mi.Linha - 1, mi.Coluna - 1] = m[mi.Linha, mi.Coluna];
    }
    
    return mint;
  }

  public static explicit operator Matriz(int[,] mint)
  {
    int _linhas = mint.GetLength(0);
    int _colunas = mint.GetLength(1);

    Matriz m = new Matriz(_linhas, _colunas);

    for(int a = 0; a < _linhas; ++a)
      for(int b = 0; b < _colunas; ++b)
        m[a + 1, b + 1] = mint[a, b];

    return m;
  }    
}
