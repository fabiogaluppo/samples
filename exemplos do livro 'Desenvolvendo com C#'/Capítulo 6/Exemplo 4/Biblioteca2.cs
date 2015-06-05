//csc /t:library Biblioteca2.cs /r:System.Windows.Forms.dll

using System.Windows.Forms;

namespace BibliotecaDeClasses
{
  public class Mensagem
  {
    public void Enviar(string sMensagem)
    {
      MessageBox.Show(sMensagem);
    }
  } 
}
