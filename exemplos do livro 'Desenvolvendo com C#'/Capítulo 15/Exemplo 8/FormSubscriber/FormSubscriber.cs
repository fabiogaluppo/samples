using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

public class FormSubscriber : Subscriber
{
  Formulário form;

  public override void Receive(string msg)
  {
    if(msg.Trim() == String.Empty) 
      form.Close();
    else
      form.DisplayMessage(msg);    
  }

  public void Initialize()
  {
    form = new Formulário();
    form.Text = "FormSubscriber";
    form.ShowDialog();    
  }

  public FormSubscriber()
  {
    ThreadStart ts = new ThreadStart(this.Initialize);
    Thread t = new Thread(ts);
    t.Start();
  }   
} 

public class Formulário : Form
{
  private string message;

  ListBox lb;
  
  public Formulário()
  {
    lb = new ListBox();
    lb.Anchor |= AnchorStyles.Right | AnchorStyles.Bottom;
    lb.Top = 30; lb.Left = 10; 
    lb.Width = ClientSize.Width - 20; lb.Height = ClientSize.Height - 40;
    lb.Font = Font;

    Font = new Font("Arial", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
   
    Controls.Add(lb);
  }  

  public void DisplayMessage(string msg)
  {
    lock(this){ message = msg; }
    
    lb.Items.Insert(0, msg);    

    Invalidate();
  }     

  protected override void OnPaint(PaintEventArgs pea)
  {
    Draw(pea.Graphics, 10, 10);
  }

  protected virtual void Draw(Graphics g, int x, int y)
  { 
    g.DrawString(message, Font, new SolidBrush(Color.Black), x, y);
  }
}