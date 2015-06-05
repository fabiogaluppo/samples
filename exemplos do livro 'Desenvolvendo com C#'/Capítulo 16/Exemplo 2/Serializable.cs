//csc Serializable.cs

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

[Serializable]
public class Correntista
{
  public string Nome;    
  public byte   Idade;
  [NonSerialized]public string SenhaCart�o;
}

public class App
{
  public static void Main()
  {
    Correntista c = new Correntista();
    c.Nome = "Fabio Galuppo";
    c.Idade = 20;
    c.SenhaCart�o = "Desenvolvendo.NET";
  
    Dump(c);
    
    FileMode create = FileMode.Create, open = FileMode.Open;
    FileAccess write = FileAccess.Write, read = FileAccess.Read;
    FileShare share = FileShare.None;
    string filebin = "correntista.bin", filexml = "correntista.soap";

    //Serializa��o Bin�ria
    IFormatter binfmtr = new BinaryFormatter();
    Stream binstrm = new FileStream(filebin, create, write, share);
    binfmtr.Serialize(binstrm, c);    
    //Serializa��o Bin�ria 

    //Serializa��o SOAP
    IFormatter soapfmtr = new SoapFormatter();
    Stream soapstrm = new FileStream(filexml, create, write, share);
    soapfmtr.Serialize(soapstrm, c);
    soapstrm.Close(); //o Soap Stream deve ser fechado
    //Serializa��o SOAP

    c = null;

    //Deserializa��o Bin�ria
    IFormatter dbinfmtr = new BinaryFormatter();
    Stream dbinstrm = new FileStream(filebin, open, read, share);
    c = (Correntista)dbinfmtr.Deserialize(dbinstrm);

     Dump(c);         
    //Deserializa��o Bin�ria 

    c = null;

    //Deserializa��o SOAP
    IFormatter dsoapfmtr = new SoapFormatter();
    Stream dsoapstrm = new FileStream(filexml, open, read, share);
    c = (Correntista)dsoapfmtr.Deserialize(dsoapstrm);
    
    Dump(c);         
    //Deserializa��o SOAP   
  }

  public static void Dump(Correntista c)
  {
    Console.WriteLine("\n{0}", c.Nome);
    Console.WriteLine(c.Idade);
    Console.WriteLine(c.SenhaCart�o);
  }
}
