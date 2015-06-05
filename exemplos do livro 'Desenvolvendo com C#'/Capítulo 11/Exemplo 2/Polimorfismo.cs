//csc Polimorfismo.cs

using System;

public class Pessoa
{
#region Atributos 

  public string Nome;
  public byte Idade;

#endregion

#region Operações

  public void Falar(string frase)
  {
    Console.WriteLine("{0} diz: {1}", Nome, frase);
  }

  public Pessoa Duplicar(Pessoa pessoa)
  {
    Pessoa novapessoa = new Pessoa();
    novapessoa.Nome = pessoa.Nome;
    novapessoa.Idade = pessoa.Idade;
   
    return novapessoa;
  }

#endregion 
}

public class Estudante : Pessoa
{
#region Atributos

  public string ID;
  public string Email;

#endregion

#region Operações

  public void Estudar(string matéria)
  {
    Console.WriteLine("{0} estuda: {1}", base.Nome, matéria); 
  }

#endregion
}

public class Polimorfismo	
{
  public static void Main()
  {
    Estudante e = new Estudante();
    e.Nome = "Fabio Galuppo";
    e.Idade = 18;
    e.ID = "ID_5678";
    e.Email = "fabiogaluppo@hotmail.com";

    Pessoa p = e; //Polimorfismo: p assume a interface Pessoa do objeto e

    p.Falar("C# é legal");

    Pessoa p2, p3;
    
    p2 = p.Duplicar(p);
    
    p.Falar("Desenvolvendo com C#");

    p3 = p.Duplicar(e); //Polimorfismo: p assume a interface Pessoa do objeto e
  
    p.Falar("OO é suportada pelo C#");

    //Cast permitido pois o objeto p foi originado de um cast do objeto e
    Estudante e2 = (Estudante)p;
    Console.WriteLine("{0} {1} {2} {3}", e2.Nome, e2.Idade, e2.ID, e2.Email);

    Pessoa p4 = new Pessoa();
    p4.Nome = "Wallace Santos";
    p4.Idade = 22;    

    //Cast não permitido pois a classe p4 não suporta todos os requisitos de e3
    Estudante e3 = (Estudante)p4; 
    
    Console.WriteLine("{0} {1}", e3.Nome, e3.Idade);
  }
}