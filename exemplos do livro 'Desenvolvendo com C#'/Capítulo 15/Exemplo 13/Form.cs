//csc Form.cs /win32res:MFCRes.res /linkres:Images.resources /nologo

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Resources;
using System.Threading;
using System.Globalization;

public class FormText : Form
{
  private Button btn_ptBR, btn_enUS, btn_itIT, btnFont;
  private Label lblText, lblTime;
  private System.Windows.Forms.Timer timer;
  private TextBox txtText;
  private FontDialog fd;
  private PictureBox picBox;
  private ResourceManager rm, rmImgs;
  private CultureInfo ci;
  
  public FormText()
  {
    ci = new CultureInfo("pt-BR");
    
    Thread.CurrentThread.CurrentUICulture = ci;
    
    rm = ResourceManager.CreateFileBasedResourceManager("Form", ".", null);
    rmImgs = new ResourceManager("Images", typeof(FormText).Assembly);

    btn_ptBR = new Button();
    btn_enUS = new Button();
    btn_itIT = new Button();
    lblText  = new Label();
    timer    = new System.Windows.Forms.Timer();
    btnFont  = new Button();
    txtText  = new TextBox();
    lblTime  = new Label();
    fd       = new FontDialog();
    picBox   = new PictureBox();
    	
    btn_ptBR.Location = new Point(32, 120);
    btn_ptBR.TabIndex = 1;
    btn_ptBR.Text = "pt-BR";

    btn_enUS.Location = new Point(112, 120);
    btn_enUS.TabIndex = 1;
    btn_enUS.Text = "en-US";

    btn_itIT.Location = new Point(192, 120);
    btn_itIT.TabIndex = 2;
    btn_itIT.Text = "it-IT";

    lblText.Location = new Point(8, 16);
    lblText.Size = new Size(280, 40);
    lblText.TabIndex = 3;
    	
    timer.Enabled = true;
    timer.Interval = 1000;
			
    btnFont.Location = new Point(8, 88);
    btnFont.TabIndex = 4;
    
    txtText.Location = new Point(8, 64);
    txtText.Size = new Size(280, 20);
    txtText.TabIndex = 0;
    	
    lblTime.BorderStyle = BorderStyle.Fixed3D;
    lblTime.Location = new Point(32, 152);
    lblTime.Size = new Size(232, 23);
    lblTime.TextAlign = ContentAlignment.MiddleCenter;
	
    fd.ShowColor = true;
    fd.ShowApply = true;

    picBox.Location = new Point(8, 184);
    picBox.Size = new Size(280, 80);
    picBox.Image = (Image)rmImgs.GetObject("VSIMG");
    picBox.SizeMode = PictureBoxSizeMode.StretchImage;
    
    ClientSize = new Size(292, 268);
    FormBorderStyle = FormBorderStyle.FixedToolWindow;

    Control[] controls = new Control[]
                           { 
                             btn_ptBR, btn_enUS, btn_itIT, lblText,                             
                             btnFont, txtText, lblTime, picBox
                           };

    Controls.AddRange(controls);
  
    timer.Tick += new EventHandler(timer_Tick);
    
    btnFont.Click += new EventHandler(btnFont_Click);
	
    fd.Apply += new EventHandler(fontDialog_Apply);

    txtText.TextChanged += new EventHandler(txtText_TextChanged);

    btn_ptBR.Click += new EventHandler(btn_Translate);
    btn_enUS.Click += new EventHandler(btn_Translate);
    btn_itIT.Click += new EventHandler(btn_Translate);

    Translate();    
  }

  public static void Main() 
  {
    Application.Run(new FormText());
  }

  private void timer_Tick(object sender, EventArgs e)
  {
    lblTime.Text = DateTime.Now.ToString(null, ci);
  }

  private void fontDialog_Apply(object sender, EventArgs e)
  {
    lblText.Font = fd.Font;
    lblText.ForeColor = fd.Color;
  }

  private void btnFont_Click(object sender, EventArgs e)
  {
    fd.Font = lblText.Font;
    fd.Color = lblText.ForeColor;
			
    if(DialogResult.OK == fd.ShowDialog())
    {
      fontDialog_Apply(this, null);
    }
			
    txtText.Focus();
  }

  private void btn_Translate(object sender, EventArgs e)
  {
    Button but = (Button)sender;

    ci = new CultureInfo(but.Text);
    Thread.CurrentThread.CurrentUICulture = ci;
    Translate();
  }

  private void txtText_TextChanged(object sender, EventArgs e)
  {
    lblText.Text = txtText.Text;
  }

  private void Translate()
  { 
    btnFont.Text = rm.GetString("btnFont.Text");
    Text = rm.GetString("$this.Text");
  }  
}

