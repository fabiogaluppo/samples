//csc Notifica.cs /r:Biblioteca1.dll /r:Biblioteca2.dll

public class Notifica
{
  public static void Main(string[] args)
  {
    //Ocorreu uma colis�o de nomes... ele usa a primeira refer�ncia
    BibliotecaDeClasses.Mensagem msg = new BibliotecaDeClasses.Mensagem();
    msg.Enviar(args[0]);
  }
}
