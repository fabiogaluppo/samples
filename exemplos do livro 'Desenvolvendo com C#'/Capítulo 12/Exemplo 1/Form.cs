//csc /target:winexe Form.cs /r:System.Windows.Forms.dll

using System;
using System.Drawing;
using System.Windows.Forms;

//Deve ser herdado para ser usado. Vísivel internamente e externamente
abstract public class FormMestre : Form
{
  //Membros

  private PictureBox logo;
  private Label texto;
		
  public FormMestre()
  {
    Inicializa();
  }
  
  private void Inicializa()
  {
    logo = new PictureBox();
    texto = new Label();

    logo.Image = Image.FromFile("Figura.JPG");
    logo.Size = new Size(37, 36);
    logo.SizeMode = PictureBoxSizeMode.AutoSize;
			
    texto.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
    texto.BackColor = Color.White;
    texto.Location = new Point(34, 0);
    texto.Size = new Size(470, 36);		
    texto.Text = "Componentware - Sistemas Componentizados";
    
    this.Controls.Add(texto);
    this.Controls.Add(logo);

  }
}

//Não pode ser herdado. Vísivel apenas internamente
sealed internal class FormSimples : FormMestre
{
  //Membros

  public FormSimples(string título) : base() //chama construtor da classe base
  {
    this.Text = título;
    this.Click += new EventHandler(this.FormSimples_Click);       
  }
  
  private void FormSimples_Click(object sender, System.EventArgs e)
  {
    MessageBox.Show("Click disparado");		
  }  
}

//Vísivel internamente e externamente
public class Aplicação
{
  //Membros

  public static void Main()
  {
    Application.Run(new FormSimples("Meu Formulário Simples"));
  }
}