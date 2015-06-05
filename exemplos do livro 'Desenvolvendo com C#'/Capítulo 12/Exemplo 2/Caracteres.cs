//csc Caracteres.cs

using System;

internal class BufferDeCaracteres
{
  public char[] buffer;                  //membro de inst�ncia
  public int posCaractereLido = 0;        //membro de inst�ncia
  static public int maxCaracteres = 256; //membro de classe (est�tico)
  public const int resetPos = 0;         //membro de classe (constante)
}

public class CaracteresSorteados
{
  public static void Main()
  {
    BufferDeCaracteres obj = new BufferDeCaracteres();
    obj.buffer = new char[BufferDeCaracteres.maxCaracteres];
    
    Random rd = new Random();
    
    while(obj.posCaractereLido < BufferDeCaracteres.maxCaracteres)
    {
      obj.buffer[obj.posCaractereLido++] = (char)rd.Next(0,255); 
    }
    
    obj.posCaractereLido = BufferDeCaracteres.resetPos;
    
    while(obj.posCaractereLido < BufferDeCaracteres.maxCaracteres)
    {  
      Console.Write(obj.buffer[obj.posCaractereLido++]);   
    }
  } 
}
