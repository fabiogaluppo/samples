//csc /target:winexe Delta.cs

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class DeltaForm : Form
{
  private int a, b, c, fontsize;
  
  #region Propriedades leitura e escrita
  public int A{ get{ return a; } set{ a = value; } }
  public int B{ get{ return b; } set{ b = value; } }
  public int C{ get{ return c; } set{ c = value; } }
  #endregion
  
  #region Propriedades somente de leitura
  public double Valor
  {
    get
    {
      return Math.Pow(b, 2) - 4 * a * c;
    }
  }  
  #endregion

  #region Propriedades somente de escrita
  public short TamanhoDaFonte
  {
    set
    {
      if(value < 0) 
        fontsize = 1;
      else if (value > 50)
        fontsize = 50; 
      else
        fontsize = value; 
    }
  }
  #endregion   

  private string fmt = "Delta = {1}\u00B2 - 4 . {0} . {2}";  

  public override string ToString()
  {
    return String.Format(fmt, a, b, c); 
  }
  
  protected override void OnPaint(PaintEventArgs pea)
  {
    Draw(pea.Graphics, 1, 25);
  }

  protected virtual void Draw(Graphics g, int x, int y)
  { 
    string text = String.Format(fmt, "a", "b", "c");
    string result = String.Format("Delta = {0}", this.Valor);

    Font font = new Font("Monotype Corsiva", fontsize, FontStyle.Italic); 
    Brush black = new SolidBrush(Color.Black);
 
    g.DrawString(text, font, black, new Point(x, y));
    
    Point p1 = new Point(x, y + fontsize + fontsize / 4);
    g.DrawString(this.ToString(), font, black, p1);
    
    Point p2 = new Point(x, y + 2 * fontsize + fontsize / 2);
    g.DrawString(result, font, black, p2);
  }

  private Label lblA, lblB, lblC;
  private TextBox txtA, txtB, txtC;  
  private Button bt;
  private ComboBox cb;

  public DeltaForm()
  {
    lblA = new Label();   lblB = new Label();   lblC = new Label();
    txtA = new TextBox(); txtB = new TextBox(); txtC = new TextBox();

    lblA.Location = new Point(10, 4); lblB.Location = new Point(60, 4);  
    lblC.Location = new Point(110, 4);
    txtA.Location = new Point(22, 2); txtB.Location = new Point(72, 2); 
    txtC.Location = new Point(122, 2);

    lblA.Width = lblB.Width = lblC.Width = 13;
    txtA.Width = txtB.Width = txtC.Width = 32;

    lblA.Text = "a"; lblB.Text = "b"; lblC.Text = "c";
    txtA.MaxLength = txtB.MaxLength = txtC.MaxLength = 4;
    
    bt = new Button();
    bt.Location = new Point(160, 2);
    bt.Height = txtA.Height; bt.Width = 80;
    bt.Text = "Recalcular";
    bt.Click += new EventHandler(this.Recalcular);

    cb = new ComboBox();
    cb.Location = new Point(245, 2);
    cb.Size = new Size(60, 21);
    cb.DropDownStyle = ComboBoxStyle.DropDownList;
    cb.SelectedIndexChanged +=  new EventHandler(this.SelecionarFonte);

    for(int a = 1; a < 51; a++) cb.Items.Add(a);
 
    cb.SelectedIndex = 15;

    Controls.AddRange(new Control[]{lblA, lblB, lblC, txtA, txtB, txtC, bt, cb});
  }

  public void Recalcular(object sender, EventArgs e)
  {
    try
    {
      A = System.Int32.Parse(txtA.Text);
      B = System.Int32.Parse(txtB.Text);
      C = System.Int32.Parse(txtC.Text);

      Text = ToString();
      Invalidate();
    }
    catch
    {
      txtA.Text = A.ToString(); 
      txtB.Text = B.ToString();
      txtC.Text = C.ToString();        
    }    
  }
  
  public void SelecionarFonte(object sender, EventArgs e)
  {
    TamanhoDaFonte = System.Int16.Parse(cb.SelectedItem.ToString());
    Invalidate();
  }

  public static void Main()
  {
    DeltaForm form = new DeltaForm();
    form.A = 1;
    form.B = 3;
    form.C = -5;
   
    form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
    form.Height = 260; form.Width = 770;
    form.Text = form.ToString();
    
    form.Recalcular(form, null);

    Application.Run(form);    
  }
}
