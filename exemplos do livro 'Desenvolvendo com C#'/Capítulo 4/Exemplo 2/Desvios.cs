//csc Desvios.cs

using System;

public class Desvios
{
  public static void Main()
  {
    bool a = true;

    goto desvio;

    //Este comando não será executado
    if(a==true)
    { 
      Console.Write("Verdadeiro");
      goto fim;
    }

     desvio: //Isto representa um rótulo
     Console.Write("Falso");

     //o rótulo é sempre seguido por um comando, neste caso um comando vazio
     fim:;     
  }
}
