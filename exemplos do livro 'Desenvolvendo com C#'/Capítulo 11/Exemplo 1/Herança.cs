//csc Heran�a.cs

using System;

public class Pessoa : ICloneable /* Heran�a de interface */
{
#region Atributos 

  public string Nome;
  public byte Idade;

#endregion

#region Opera��es

  public void Falar(string frase)
  {
    Console.WriteLine("{0} diz: {1}", Nome, frase);
  }

  //Implementado o m�todo clone da interface ICloneable
  public object Clone()
  {
    Pessoa clone = new Pessoa();
    clone.Nome = String.Format("Clone {0}", Nome);
    clone.Idade = Idade;

    return clone; 
  }

#endregion 
}

public class Estudante : Pessoa /* Heran�a de implementa��o */
{
#region Atributos

  public string ID;
  public string Email;

#endregion

#region Opera��es

  public void Estudar(string mat�ria)
  {
    Console.WriteLine("{0} estuda: {1}", base.Nome, mat�ria); 
  }

  public new object Clone()
  {
    Estudante clone = new Estudante();
    Pessoa p = (Pessoa)base.Clone();
    clone.Nome = p.Nome;
    clone.Idade = p.Idade;
    clone.ID = ID;
    clone.Email = Email;

    return clone;
  }

#endregion
}

public class Heran�a	
{
  public static void Main()
  {
    Pessoa p = new Pessoa();
    p.Nome = "Fabio Galuppo";
    p.Idade = 18;

    Pessoa pclone = (Pessoa)p.Clone();

    p.Falar("C# � legal");
    pclone.Falar("C# suporta Aspectos");

    Estudante e = new Estudante();
    e.Nome = "Vanclei Matheus";
    e.Idade = 20;
    e.ID = "EST_1234";
    e.Email = "vanclei@desenvolvendo.net";

    Estudante eclone = (Estudante)e.Clone();
    
    e.Estudar("C#");
    eclone.Estudar("Composi��o");    
  }
}
