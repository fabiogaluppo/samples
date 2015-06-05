//csc XmlException.cs /r:System.Xml.dll

using System;
using System.Xml;

public class XmlException
{
  public static void Main()
  {
    XmlDocument doc = null;

    try
    {	
      doc = new XmlDocument();                             
      doc.LoadXml("<Exception>The Exception</Exception>"); //Carrega o conte�do       
   
      throw new Exception(doc.InnerText); //Dispara a exce��o
    }
    catch(OutOfMemoryException)
    {  
      //Tratamento aqui
    }
    catch(NullReferenceException)
    {   
      //Tratamento aqui
    }
    catch(Exception e)
    {
      //Tratamento aqui
      Console.WriteLine("Exce��o ocorrida no programa {0}", e);
    }
    finally
    {
      Console.WriteLine(@"Gravando o Documento..."); //Uso do verbatim (@)
      doc.Save(@"c:\exception.xml"); //Grava o conte�do 
    }    
  }
}
