//csc /target:winexe Cores.cs
//csc /target:winexe /noconfig Cores.cs /r:System.Windows.Forms.dll /r:System.Drawing.dll /System.dll

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class Formulário : Form
{
  protected override void OnPaint(PaintEventArgs pea)
  {
    Draw(pea.Graphics, ClientSize.Width, ClientSize.Height);
  }

  protected virtual void Draw(Graphics g, int dx, int dy)
  { 
    Random rand = new Random(); 
    for(int a = 0, l = (dx * dy) / 10; a < l; ++a)
    {
      int x = rand.Next(0, dx);
      int y = rand.Next(0, dy);
		
      int red = rand.Next(byte.MinValue, byte.MaxValue);
      int green = rand.Next(byte.MinValue, byte.MaxValue);
      int blue = rand.Next(byte.MinValue, byte.MaxValue);
			  
      Brush brush = new SolidBrush(Color.FromArgb(red, green, blue));

      g.FillRectangle(brush, x, y, 10, 10);
    }
   
    string text = "Desenvolvendo com C#";

    Font font = new Font("Verdana", 50, FontStyle.Bold); 

    SizeF size = g.MeasureString(text, font);

    PointF point = new PointF((dx - size.Width) / 2, (dy - size.Height) / 2); 

    RectangleF rect = new RectangleF(point, size);
    
    LinearGradientBrush gbrush = new LinearGradientBrush(rect, Color.Red, Color.Black, LinearGradientMode.ForwardDiagonal);

    g.DrawString(text, font, gbrush, point);        
  }
  
  public static void Main()
  {
    Formulário form = new Formulário();
    form.WindowState = FormWindowState.Maximized;
    form.Text = "Cores";
    Application.Run(form);    
  }
}