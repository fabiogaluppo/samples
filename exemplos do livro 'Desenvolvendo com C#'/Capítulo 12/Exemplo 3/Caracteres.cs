//csc Caracteres.cs

using System;

internal class BufferDeCaracteres
{
  private int posCaractereLido; //membro de instância
  private static int resetPos;  //membro de classe
  private char[] buffer;        //membro de instância

  static BufferDeCaracteres() //membro de classe (construtor)
  { 
    resetPos = 0; 
  } 
  
  private BufferDeCaracteres(int n) //membro de instância (construtor)
  { 
    buffer = new char[n]; 
    posCaractereLido = 0; 
  }  
    
  private void VerificarLimiteDoBuffer(int idx) //membro de instância
  {
    if(idx < 0 && idx > buffer.Length - 1)    
      throw new IndexOutOfRangeException(); 
  }

  public char PegarCaractere(int pos) //membro de instância
  { 
    VerificarLimiteDoBuffer(pos);
    
    return buffer[pos];    
  }

  public void AtribuirCaractere(int pos, char caractere) //membro de instância
  {
    VerificarLimiteDoBuffer(pos);

    buffer[pos] = caractere;
  }

  public char PróximoCaractere() //membro de instância
  {
    char ch = (char)0x0;
    return posCaractereLido < buffer.Length ? buffer[posCaractereLido++] : ch;
  }

  public void Reset() //membro de instância
  {
    posCaractereLido = BufferDeCaracteres.resetPos;
  }

  public int LarguraBuffer() //membro de instância
  {
    return buffer.Length;
  }

  public static BufferDeCaracteres CreateInstance(int n) //membro de classe
  {
    return new BufferDeCaracteres(n);
  }
}

public class CaracteresSorteados
{
  public static void Main()
  {
    BufferDeCaracteres obj = BufferDeCaracteres.CreateInstance(256);   

    Random rd = new Random();
    
    for(int a = 0, l = obj.LarguraBuffer(); a < l; ++a)    
    {
      obj.AtribuirCaractere(a, (char)rd.Next(1, 255)); 
    }
    
    obj.Reset();
    
    char ch;
    while((ch = obj.PróximoCaractere()) != 0x0)
    {  
      Console.Write(ch);   
    }
  } 
}