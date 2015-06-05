//csc Aritmetica.cs

using System;

public class Aritmetica
{
  public static void Main(string[] args)
  {
    //Verifica o número de argumentos entrados
    if(args.Length == 3)
    {	
      int x = 0, y = 0;
	
      //Convertem os valores dos argumentos 2 e 3 para inteiro 32-bit
      //Se ocorrer algum erro o modo de utilização	
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

      //Efetua a operação selecionada no primeiro argumento
      switch(args[0])
      {
        case "+":
          Console.Write("Valor da soma = {0}", x + y);
          break;
        case "-":
          Console.Write("Valor da subtração = {0}", x - y);
          break;
        case "/":
          Console.Write("Valor da divisão = {0}", x / y);
          break;
        case "*":
          Console.Write("Valor da multiplicação = {0}", x * y);
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
    //Modo de utilização
    Console.WriteLine("Modo de usar: Aritmetica operador valor1 valor2");
    Console.WriteLine("Ex.: Aritmetica + 100 200");
  }
}
