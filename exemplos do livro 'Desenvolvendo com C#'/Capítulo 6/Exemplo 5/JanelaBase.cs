//csc /t:library /nologo JanelaBase.cs

using System;

namespace Graficos
{
  public abstract class JanelaBase
  {
    private char se, s, sd, le, ld, ie, id, i;

    protected char SuperiorEsquerda{ set { se = value; } }
    protected char Superior{ set { s = value; } }
    protected char SuperiorDireita{ set { sd = value; } }
    protected char LateralEsquerda{ set { le = value; } }
    protected char LateralDireita{ set { ld = value; } }
    protected char InferiorEsquerda{ set { ie = value; } }
    protected char Inferior{ set { i = value; } }
    protected char InferiorDireita{ set { id = value; } } 
    
    public JanelaBase(){ Largura = Altura = 10; }
    
    public byte Altura, Largura;

    public void Desenhar()
    {
      Console.Write("\n{0}", se);
      for(int a = 1, l = Largura - 1; a < l; a++)
	Console.Write(s);
      Console.Write(sd);

      for(int b = 1, h = Altura - 1; b < h; b++)
      {
        Console.Write("\n{0}", le);
        for(int a = 1, l = Largura - 1; a < l; a++)
	  Console.Write(" ");
        Console.Write(ld);
      }

      Console.Write("\n{0}", ie);
      for(int a = 1, l = Largura - 1; a < l; a++)
	Console.Write(i);
      Console.Write("{0}\n", id);      
    }    
  }  
}