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
	private DateTime datacria��o;

    private string[] pap�is;

    public UsrType(string nome, string senha, string[] pap�is, bool ativo)
    {
      this.nome = nome;
      this.senha = senha;
      this.ativo = ativo;
      this.pap�is = pap�is; 
      datacria��o = DateTime.Now;
    }

    public string Nome { get{ return nome;  } }
    public bool Ativo{ get{ return ativo; } set{ ativo = value; } }  
    public string[] Pap�is{ get{ return pap�is; } }
    public DateTime DataCria��o{ get{ return datacria��o; } }
	internal string Senha{ get{ return senha; } set{ senha = value; } }
  }
  
  public class Usu�rio
  {
    public void AlterarSenha(UsrType u, string senhaAntiga, string senhaNova)
	{
	  if(senhaAntiga != u.Senha)
		throw new ArgumentException("Senha antiga inv�lida");
    
	  u.Senha = senhaNova;
	}

	public GenericPrincipal CriarPrincipal(UsrType u)
	{
	  return new GenericPrincipal(new GenericIdentity(u.Nome), u.Pap�is); 
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
    Stateless.Usu�rio usr = new Stateless.Usu�rio();  

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
    string filebin = "Usu�rio.bin", filexml = "Usu�rio.soap";

    //Serializa��o Bin�ria
    IFormatter binfmtr = new BinaryFormatter();
    Stream binstrm = new FileStream(filebin, create, write, share);
    binfmtr.Serialize(binstrm, u);    
    //Serializa��o Bin�ria 

    //Serializa��o SOAP
    IFormatter soapfmtr = new SoapFormatter();
    Stream soapstrm = new FileStream(filexml, create, write, share);
    soapfmtr.Serialize(soapstrm, u);
    soapstrm.Close(); //o Soap Stream deve ser fechado
    //Serializa��o SOAP

    //Deserializa��o Bin�ria
    IFormatter dbinfmtr = new BinaryFormatter();
    Stream dbinstrm = new FileStream(filebin, open, read, share);
    Stateless.UsrType usrb = (Stateless.UsrType)dbinfmtr.Deserialize(dbinstrm);
    Dump(usrb);     
    //Deserializa��o Bin�ria 

    //Deserializa��o SOAP
    IFormatter dsoapfmtr = new SoapFormatter();
    Stream dsoapstrm = new FileStream(filexml, open, read, share);
    Stateless.UsrType usrs = (Stateless.UsrType)dsoapfmtr.Deserialize(dsoapstrm);
    Dump(usrs);     
    //Deserializa��o SOAP   
  }

  public static void Dump(Stateless.UsrType u)
  {
    Console.WriteLine("\n{0}", u.Nome);
    Console.WriteLine(u.Ativo);
    Console.WriteLine(u.DataCria��o);
    foreach(string r in u.Pap�is) Console.WriteLine(r);    
  }
}