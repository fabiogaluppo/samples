//csc Stateless.cs

using System;
using System.IO;
using System.Security.Principal;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace Stateless
{
  [Serializable]
  public class UsrType
  {
    private string nome;    
    private bool ativo;
    private string senha;
	private DateTime datacriação;

    private string[] papéis;

    public UsrType(string nome, string senha, string[] papéis, bool ativo)
    {
      this.nome = nome;
      this.senha = senha;
      this.ativo = ativo;
      this.papéis = papéis; 
      datacriação = DateTime.Now;
    }

    public string Nome { get{ return nome;  } }
    public bool Ativo{ get{ return ativo; } set{ ativo = value; } }  
    public string[] Papéis{ get{ return papéis; } }
    public DateTime DataCriação{ get{ return datacriação; } }
	internal string Senha{ get{ return senha; } set{ senha = value; } }
  }
  
  public class Usuário
  {
    public void AlterarSenha(UsrType u, string senhaAntiga, string senhaNova)
	{
	  if(senhaAntiga != u.Senha)
		throw new ArgumentException("Senha antiga inválida");
    
	  u.Senha = senhaNova;
	}

	public GenericPrincipal CriarPrincipal(UsrType u)
	{
	  return new GenericPrincipal(new GenericIdentity(u.Nome), u.Papéis); 
	}
  }
}

public class App
{
  public static void Main()
  {
    string[] rls = new string[]{ "Administradores", "Desenvolvedores" };
    string name = "Fabio Galuppo";
    string pwd = "SenhaSecreta";

    Stateless.UsrType u = new Stateless.UsrType(name, pwd, rls, false);
    Stateless.Usuário usr = new Stateless.Usuário();  

    usr.AlterarSenha(u, "SenhaSecreta", "<SenhaSecreta.752 />");

    GenericPrincipal pcp = usr.CriarPrincipal(u);

    if(pcp.IsInRole("Administradores"))
    {
      u.Ativo = true;
    }
   
    Dump(u);
    
    FileMode create = FileMode.Create, open = FileMode.Open;
    FileAccess write = FileAccess.Write, read = FileAccess.Read;
    FileShare share = FileShare.None;
    string filebin = "Usuário.bin", filexml = "Usuário.soap";

    //Serialização Binária
    IFormatter binfmtr = new BinaryFormatter();
    Stream binstrm = new FileStream(filebin, create, write, share);
    binfmtr.Serialize(binstrm, u);    
    //Serialização Binária 

    //Serialização SOAP
    IFormatter soapfmtr = new SoapFormatter();
    Stream soapstrm = new FileStream(filexml, create, write, share);
    soapfmtr.Serialize(soapstrm, u);
    soapstrm.Close(); //o Soap Stream deve ser fechado
    //Serialização SOAP

    //Deserialização Binária
    IFormatter dbinfmtr = new BinaryFormatter();
    Stream dbinstrm = new FileStream(filebin, open, read, share);
    Stateless.UsrType usrb = (Stateless.UsrType)dbinfmtr.Deserialize(dbinstrm);
    Dump(usrb);     
    //Deserialização Binária 

    //Deserialização SOAP
    IFormatter dsoapfmtr = new SoapFormatter();
    Stream dsoapstrm = new FileStream(filexml, open, read, share);
    Stateless.UsrType usrs = (Stateless.UsrType)dsoapfmtr.Deserialize(dsoapstrm);
    Dump(usrs);     
    //Deserialização SOAP   
  }

  public static void Dump(Stateless.UsrType u)
  {
    Console.WriteLine("\n{0}", u.Nome);
    Console.WriteLine(u.Ativo);
    Console.WriteLine(u.DataCriação);
    foreach(string r in u.Papéis) Console.WriteLine(r);    
  }
}