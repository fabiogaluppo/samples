//csc /nologo Janelas.cs /r:Janela1.dll /r:Janela2.dll /r:JanelaBase.dll

public class Janelas
{
  public static void Main()
  {
    Graficos.JanelaBordaSimples janela1 = new Graficos.JanelaBordaSimples();
    janela1.Desenhar();
    Graficos.JanelaBordaDupla janela2 = new Graficos.JanelaBordaDupla();
    janela2.Desenhar();
	
    janela1.Largura = 5;
    janela1.Altura = 5;
    janela1.Desenhar();
    janela2.Largura = 50;
    janela2.Altura = 12;
    janela2.Desenhar();
  }
}