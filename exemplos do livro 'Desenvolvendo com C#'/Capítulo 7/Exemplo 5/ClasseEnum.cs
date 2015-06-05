//csc ClasseEnum.cs

using System;

public class ClasseEnum
{
  enum Dia
  {
    segunda,
    terça,
    quarta,
    quinta,
    sexta,
    sábado,
    domingo
  }
  
  public static void Main()
  {
    Dia dia = Dia.domingo;

    Console.WriteLine(Enum.GetName(typeof(Dia), 5));

    foreach(string enumerador in Enum.GetNames(typeof(Dia)))
      Console.Write("{0} ", enumerador);

    Console.WriteLine("");

    int[] valores = (int[])Enum.GetValues(typeof(Dia));
    for(int a = 0, l = valores.Length; a < l; ++a)
      Console.Write("{0} ", valores[a]);

    Console.WriteLine("\n{0}", Enum.IsDefined(typeof(Dia), "segunda"));  
    
    Console.WriteLine("{0}", Enum.IsDefined(typeof(Dia), "teste"));  

    dia = (Dia)Enum.Parse(typeof(Dia), "segunda");
    Console.WriteLine(dia.ToString());

    dia = (Dia)Enum.Parse(typeof(Dia), "3");
    Console.WriteLine(dia.ToString());
  }
}