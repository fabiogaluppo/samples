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
  public class Usuário
  {
    private string nome;
    private string senha;
    private bool ativo;
    private DateTime datacriação;

    private string[] papéis;

    public Usuário(string nome, string senha, string[] papéis, bool ativo)
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

    public void AlterarSenha(string senhaAntiga, string senhaNova)
    {
      if(senhaAntiga != senha)
        throw new ArgumentException("Senha antiga inválida");
    
      senha = senhaNova;
    }

    public GenericPrincipal CriarPrincipal()
    {
      return new GenericPrincipal(new GenericIdentity(nome), papéis); 
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

    Statefull.Usuário usr = new Statefull.Usuário(name, pwd, rls, false);
      
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
    string filebin = "Usuário.bin", filexml = "Usuário.soap";

    //Serialização Binária
    IFormatter binfmtr = new BinaryFormatter();
    Stream binstrm = new FileStream(filebin, create, write, share);
    binfmtr.Serialize(binstrm, usr);    
    //Serialização Binária 

    //Serialização SOAP
    IFormatter soapfmtr = new SoapFormatter();
    Stream soapstrm = new FileStream(filexml, create, write, share);
    soapfmtr.Serialize(soapstrm, usr);
    soapstrm.Close(); //o Soap Stream deve ser fechado
    //Serialização SOAP

    //Deserialização Binária
    IFormatter dbinfmtr = new BinaryFormatter();
    Stream dbinstrm = new FileStream(filebin, open, read, share);
    Statefull.Usuário usrb = (Statefull.Usuário)dbinfmtr.Deserialize(dbinstrm);
    Dump(usrb);     
    //Deserialização Binária 

    //Deserialização SOAP
    IFormatter dsoapfmtr = new SoapFormatter();
    Stream dsoapstrm = new FileStream(filexml, open, read, share);
    Statefull.Usuário usrs = (Statefull.Usuário)dsoapfmtr.Deserialize(dsoapstrm);
    Dump(usrs);     
    //Deserialização SOAP   
  }

  public static void Dump(Statefull.Usuário usr)
  {
    Console.WriteLine("\n{0}", usr.Nome);
    Console.WriteLine(usr.Ativo);
    Console.WriteLine(usr.DataCriação);
    foreach(string r in usr.Papéis) Console.WriteLine(r);    
  }
}