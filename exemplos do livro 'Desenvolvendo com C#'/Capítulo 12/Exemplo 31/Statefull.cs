//csc Statefull.cs

using System;
using System.IO;
using System.Security.Principal;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace Statefull
{
  [Serializable]
  public class Usu�rio
  {
    private string nome;
    private string senha;
    private bool ativo;
    private DateTime datacria��o;

    private string[] pap�is;

    public Usu�rio(string nome, string senha, string[] pap�is, bool ativo)
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

    public void AlterarSenha(string senhaAntiga, string senhaNova)
    {
      if(senhaAntiga != senha)
        throw new ArgumentException("Senha antiga inv�lida");
    
      senha = senhaNova;
    }

    public GenericPrincipal CriarPrincipal()
    {
      return new GenericPrincipal(new GenericIdentity(nome), pap�is); 
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

    Statefull.Usu�rio usr = new Statefull.Usu�rio(name, pwd, rls, false);
      
    usr.AlterarSenha("SenhaSecreta", "<SenhaSecreta.752 />");

    GenericPrincipal pcp = usr.CriarPrincipal();

    if(pcp.IsInRole("Administradores"))
    {
      usr.Ativo = true;
    }
   
    Dump(usr);
    
    FileMode create = FileMode.Create, open = FileMode.Open;
    FileAccess write = FileAccess.Write, read = FileAccess.Read;
    FileShare share = FileShare.None;
    string filebin = "Usu�rio.bin", filexml = "Usu�rio.soap";

    //Serializa��o Bin�ria
    IFormatter binfmtr = new BinaryFormatter();
    Stream binstrm = new FileStream(filebin, create, write, share);
    binfmtr.Serialize(binstrm, usr);    
    //Serializa��o Bin�ria 

    //Serializa��o SOAP
    IFormatter soapfmtr = new SoapFormatter();
    Stream soapstrm = new FileStream(filexml, create, write, share);
    soapfmtr.Serialize(soapstrm, usr);
    soapstrm.Close(); //o Soap Stream deve ser fechado
    //Serializa��o SOAP

    //Deserializa��o Bin�ria
    IFormatter dbinfmtr = new BinaryFormatter();
    Stream dbinstrm = new FileStream(filebin, open, read, share);
    Statefull.Usu�rio usrb = (Statefull.Usu�rio)dbinfmtr.Deserialize(dbinstrm);
    Dump(usrb);     
    //Deserializa��o Bin�ria 

    //Deserializa��o SOAP
    IFormatter dsoapfmtr = new SoapFormatter();
    Stream dsoapstrm = new FileStream(filexml, open, read, share);
    Statefull.Usu�rio usrs = (Statefull.Usu�rio)dsoapfmtr.Deserialize(dsoapstrm);
    Dump(usrs);     
    //Deserializa��o SOAP   
  }

  public static void Dump(Statefull.Usu�rio usr)
  {
    Console.WriteLine("\n{0}", usr.Nome);
    Console.WriteLine(usr.Ativo);
    Console.WriteLine(usr.DataCria��o);
    foreach(string r in usr.Pap�is) Console.WriteLine(r);    
  }
}