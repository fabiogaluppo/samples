//csc /t:library /baseaddress:0x22220000 CopiarGUI.cs

using System;
using System.Drawing;
using System.Windows.Forms;

public class Formulário : Form
{
  public Button botão;
  public ProgressBar progresso;
  public bool IsCanceled;
  
  public Formulário()
  {
    botão = new Button();
    progresso = new ProgressBar();
   
    botão.DialogResult = DialogResult.Cancel;
    botão.Location = new Point(184, 40);
    botão.Size = new Size(88, 23);
    botão.Text = "&Cancelar";
    
    progresso.Location = new Point(8, 8);
    progresso.Size = new Size(264, 23);

    CancelButton = botão;
    ClientSize = new Size(280, 68);
    FormBorderStyle = FormBorderStyle.Fixed3D;
    Controls.AddRange(new Control[]{progresso, botão});

    botão.Click += new EventHandler(botão_Cancelado);
  }

  public void botão_Cancelado(object sender, EventArgs e)
  {
    IsCanceled = true;
  }  
}
