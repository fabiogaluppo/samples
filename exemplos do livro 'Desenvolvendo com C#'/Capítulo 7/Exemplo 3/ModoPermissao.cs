//csc ModoPermissao.cs

using System;

[Flags]
public enum ModoDePermissão
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
    ModoDePermissão mp = ModoDePermissão.Ler | ModoDePermissão.Escrever; //0x3
    Console.WriteLine(mp.ToString()); //Ler, Escrever
    Console.WriteLine("Permissões : ");
    Console.Write("Ler = {0}, ", (mp & ModoDePermissão.Ler) != 0);
    Console.Write("Escrever = {0}, ", (mp & ModoDePermissão. Escrever) != 0);
    Console.Write("Pesquisar = {0}, ", (mp & ModoDePermissão. Pesquisar) != 0);
    Console.Write("Eliminar = {0}", (mp & ModoDePermissão. Eliminar) != 0);
  }

  public static void Main()
  {
    Aplicacao app = new Aplicacao();
    app.Processar();
  }
}
