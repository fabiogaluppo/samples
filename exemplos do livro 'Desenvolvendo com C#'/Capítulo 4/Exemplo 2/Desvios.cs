//csc Desvios.cs

using System;

public class Desvios
{
  public static void Main()
  {
    bool a = true;

    goto desvio;

    //Este comando n�o ser� executado
    if(a==true)
    { 
      Console.Write("Verdadeiro");
      goto fim;
    }

     desvio: //Isto representa um r�tulo
     Console.Write("Falso");

     //o r�tulo � sempre seguido por um comando, neste caso um comando vazio
     fim:;     
  }
}
