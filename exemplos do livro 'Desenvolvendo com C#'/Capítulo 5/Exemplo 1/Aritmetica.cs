//csc Aritmetica.cs

using System;

public class Aritmetica
{
  public static void Main(string[] args)
  {
    //Verifica o n�mero de argumentos entrados
    if(args.Length == 3)
    {	
      int x = 0, y = 0;
	
      //Convertem os valores dos argumentos 2 e 3 para inteiro 32-bit
      //Se ocorrer algum erro o modo de utiliza��o	
      try
      {
        x = Convert.ToInt32(args[1]); 
        y = Convert.ToInt32(args[2]); 
      }
      catch
      {
        ModoUsar();
        return;	
      }

      //Efetua a opera��o selecionada no primeiro argumento
      switch(args[0])
      {
        case "+":
          Console.Write("Valor da soma = {0}", x + y);
          break;
        case "-":
          Console.Write("Valor da subtra��o = {0}", x - y);
          break;
        case "/":
          Console.Write("Valor da divis�o = {0}", x / y);
          break;
        case "*":
          Console.Write("Valor da multiplica��o = {0}", x * y);
          break;
        case "%":
          Console.Write("Valor da sobra = {0}", x % y);
          break;
        default:
          ModoUsar();
          return;
      }
    }
    else
    {  
      ModoUsar();
    }
  }

  public static void ModoUsar()
  {  
    //Modo de utiliza��o
    Console.WriteLine("Modo de usar: Aritmetica operador valor1 valor2");
    Console.WriteLine("Ex.: Aritmetica + 100 200");
  }
}
