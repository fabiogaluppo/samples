//csc ModoPermissao.cs

using System;

[Flags]
public enum ModoDePermiss�o
{
  Ler = 0x1,
  Escrever = 0x2,
  Pesquisar = 0x4,
  Eliminar = 0x8
}

public class Aplicacao
{
  public void Processar()
  {
    ModoDePermiss�o mp = ModoDePermiss�o.Ler | ModoDePermiss�o.Escrever; //0x3
    Console.WriteLine(mp.ToString()); //Ler, Escrever
    Console.WriteLine("Permiss�es : ");
    Console.Write("Ler = {0}, ", (mp & ModoDePermiss�o.Ler) != 0);
    Console.Write("Escrever = {0}, ", (mp & ModoDePermiss�o. Escrever) != 0);
    Console.Write("Pesquisar = {0}, ", (mp & ModoDePermiss�o. Pesquisar) != 0);
    Console.Write("Eliminar = {0}", (mp & ModoDePermiss�o. Eliminar) != 0);
  }

  public static void Main()
  {
    Aplicacao app = new Aplicacao();
    app.Processar();
  }
}
