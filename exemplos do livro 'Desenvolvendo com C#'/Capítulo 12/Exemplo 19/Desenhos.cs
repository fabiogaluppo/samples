//csc /target:winexe Desenhos.cs /r:System.Windows.Forms.dll

using System;
using System.Drawing;
using System.Windows.Forms;

public class Ponto
{
  public int x1, y1;

  public virtual void Desenhar(Graphics g)
  {
    Pen pen = new Pen(Color.Black);
    g.DrawLine(pen, x1, y1, x1 + .1f, y1);     
  }
} 

public class Linha : Ponto
{
  public int x2, y2;

  public override void Desenhar(Graphics g)
  {
    Pen pen = new Pen(Color.Black);
    g.DrawLine(pen, x1, y1, x2, y2);
  }
}

public class Triângulo : Linha
{
  public int x3, y3;

  public override void Desenhar(Graphics g)
  {
    Point[] points = 
    {
      new Point(x1, y1),
      new Point(x2, y2),
      new Point(x3, y3),
      new Point(x1, y1)
    };
   
    Pen pen = new Pen(Color.Black);
    g.DrawPolygon(pen, points);
  }
}

public class Formulário : Form
{
  private Ponto ponto;
  private Linha linha;
  private Triângulo triângulo;
		
  public Formulário()
  {
    Inicializa();
  }
  
  private void Inicializa()
  {
    ponto = new Ponto();
    ponto.x1 = 100;
    ponto.y1 = 30;

    linha = new Linha();
    linha.x1 = 130;
    linha.x2 = 130;
    linha.y1 = 130;
    linha.y2 = 230;

    triângulo = new Triângulo();
    triângulo.x1 = 200;
    triângulo.x2 = 230;
    triângulo.x3 = 215;
    triângulo.y1 = 200;
    triângulo.y2 = 200;
    triângulo.y3 = 115;

    this.Text = "Meu Formulário";
  }
  
  protected void Pintar(Graphics g, Ponto p)
  {
    p.Desenhar(g); 
  }
  
  protected override void OnPaint(PaintEventArgs e)
  {
    Pintar(e.Graphics, ponto);
    Pintar(e.Graphics, linha);
    Pintar(e.Graphics, triângulo);    
  }
  
  public static void Main()
  {
    Formulário form = new Formulário();
    Application.Run(form);    
  }
}