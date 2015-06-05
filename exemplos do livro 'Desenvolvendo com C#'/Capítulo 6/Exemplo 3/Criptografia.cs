//csc Criptografia.cs /r:System.Security.dll

using CP = System.Security.Cryptography; //using como alias
using IO = System.IO;                    //using como alias
using System;                            //using para encurtar o caminho

public class Criptografia
{
  public static void Main()
  {
    //chave secreta
    byte[] Key = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};
    //vetor de inicializa��o
    byte[] IV = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};

    //Stream de mem�ria
    IO.MemoryStream memstream = new IO.MemoryStream(15);
    //Stream de criptografia
    CP.RC2CryptoServiceProvider provider = new CP.RC2CryptoServiceProvider();
    CP.ICryptoTransform transform = provider.CreateEncryptor(Key, IV);
    CP.CryptoStreamMode mode = CP.CryptoStreamMode.Write;
    CP.CryptoStream stream = new CP.CryptoStream(memstream, transform, mode);
         
    //L� cada caracter da string
    foreach(char ch in "Isto � um teste")
    { 
      stream.WriteByte((Convert.ToByte(ch)));
    }
			
    int c;
			
    //Reposiciona o ponteiro para leitura
    memstream.Position = c = 0; //t�cnica n�o trivial, mas v�lida 

    while((c = memstream.ReadByte()) != -1)
    {
      Console.Write((char)c);
    }			

    stream.Close(); //Libera a stream (crypto)
    memstream.Close(); //Libera a stream (mem)  
  } 
}