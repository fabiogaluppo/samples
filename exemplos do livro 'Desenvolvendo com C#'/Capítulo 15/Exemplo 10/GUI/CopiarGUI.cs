//csc /t:library /baseaddress:0x22220000 CopiarGUI.cs

using System;
using System.Drawing;
using System.Windows.Forms;

public class Formul�rio : Form
{
  public Button bot�o;
  public ProgressBar progresso;
  public bool IsCanceled;
  
  public Formul�rio()
  {
    bot�o = new Button();
    progresso = new ProgressBar();
   
    bot�o.DialogResult = DialogResult.Cancel;
    bot�o.Location = new Point(184, 40);
    bot�o.Size = new Size(88, 23);
    bot�o.Text = "&Cancelar";
    
    progresso.Location = new Point(8, 8);
    progresso.Size = new Size(264, 23);

    CancelButton = bot�o;
    ClientSize = new Size(280, 68);
    FormBorderStyle = FormBorderStyle.Fixed3D;
    Controls.AddRange(new Control[]{progresso, bot�o});

    bot�o.Click += new EventHandler(bot�o_Cancelado);
  }

  public void bot�o_Cancelado(object sender, EventArgs e)
  {
    IsCanceled = true;
  }  
}
