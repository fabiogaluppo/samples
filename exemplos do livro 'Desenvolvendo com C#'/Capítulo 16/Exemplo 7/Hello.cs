//csc Hello.cs AssemblyInfo.cs

using System;
using System.Xml;

public class HelloWorldConnected
{
  public static void Main()
  {
    XmlTextWriter writer = new XmlTextWriter(Console.Out);
    
    writer.Formatting = Formatting.Indented;
    
    Console.WriteLine("");
    
    writer.WriteStartElement("devnet","Message", "www.desenvolvendo.net");
    writer.WriteStartAttribute("Comments", null);
    writer.WriteString("Hello World of Internet Programmable Era");
    writer.WriteEndAttribute();
    writer.WriteString("Hello, World !!!");
    writer.WriteEndElement();    

    writer.Close();    
  }
}