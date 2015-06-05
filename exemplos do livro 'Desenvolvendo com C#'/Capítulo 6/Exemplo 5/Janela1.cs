//csc /t:library /nologo Janela1.cs /r:JanelaBase.dll

using System;

namespace Graficos
{
  public class JanelaBordaSimples : JanelaBase
  {
    public JanelaBordaSimples()
    {
      this.SuperiorEsquerda = '\u250c';
      this.Superior = '\u2500';
      this.SuperiorDireita = '\u2510';
      this.LateralEsquerda = '\u2502';
      this.LateralDireita = '\u2502';
      this.InferiorEsquerda = '\u2514';
      this.Inferior = '\u2500';
      this.InferiorDireita = '\u2518'; 
    }    
  }  
}