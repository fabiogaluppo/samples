//csc Notifica.cs /r:Biblioteca1.dll /r:Biblioteca2.dll

public class Notifica
{
  public static void Main(string[] args)
  {
    //Ocorreu uma colisão de nomes... ele usa a primeira referência
    BibliotecaDeClasses.Mensagem msg = new BibliotecaDeClasses.Mensagem();
    msg.Enviar(args[0]);
  }
}
