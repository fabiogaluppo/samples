//csc /t:library /nologo Janela2.cs /r:JanelaBase.dll

namespace Graficos
{
  public class JanelaBordaDupla : JanelaBase
  {
    public JanelaBordaDupla()
    {
      this.SuperiorEsquerda = '\u2554';
      this.Superior = '\u2550';
      this.SuperiorDireita = '\u2557';
      this.LateralEsquerda = '\u2551';
      this.LateralDireita = '\u2551';
      this.InferiorEsquerda = '\u255a';
      this.Inferior = '\u2550';
      this.InferiorDireita = '\u255d'; 
    }    
  }  
}