//csc Copiar.cs

using System;
using System.IO;

public class Copiar
{
  public static int Main(string[] argumentos)
  {
    //Verifica se o número de argumentos é menor que 2
    if(argumentos.Length < 2)
    {
      ModoUsar(); //Exibe modo de usar o aplicativo
      return 1; //Status mode de uso disparado
    }

    //Verifica se o arquivo origem existe
    if(!VerificarSeArquivoExiste(argumentos[0]))
      return 2; //Status arquivo não existente

    return CopiarArquivo(argumentos[1], argumentos[0]); //Status 3 erro     
  }
 
  private static void ModoUsar()
  {
    Console.WriteLine("Modo de usar: Copiar arquivo_origem arquivo_destino");
  }

  private static bool VerificarSeArquivoExiste(string nomeArquivo)
  {
    return File.Exists(nomeArquivo);
  }

  private static int CopiarArquivo(string arquivoDestino, string arquivoOrigem)
  {
    FileStream fsOrigem, fsDestino;
    try
    {
      //Abre arquivo origem para leitura
      fsOrigem = new FileStream(arquivoOrigem, FileMode.Open);
      
      //Abre arquivo destino para escrita
      fsDestino = new FileStream(arquivoDestino, FileMode.Create);

      long tamanhoArquivo = fsOrigem.Length; //Tamanho do arquivo origem

      int byteLido = 0;

      long passo = fsOrigem.Length / 20;
      long imprimir = 0; 

      Console.Write("Cópia 0% \u25A0");
      
      //Lê e escreve byte por byte do arquivo origem para o arquivo destino
      while((byteLido = fsOrigem.ReadByte()) != -1)
      {
        if(passo == ++imprimir)
        {  
          Console.Write("\u25A0");
          imprimir = 0;
        }

        fsDestino.WriteByte((byte)byteLido);
      }

      Console.Write(" 100%");
    }
    catch(Exception e)
    {
      Console.WriteLine("Mensagem de Erro : {0}", e);
      return 3;
    }
    
    return 0;     
  }
}
