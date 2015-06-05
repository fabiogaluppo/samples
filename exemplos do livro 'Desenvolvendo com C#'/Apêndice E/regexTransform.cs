//csc regexTransform.cs
//clix regexTransform.exe

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;

public class regexTransform
{
  public static void Main()
  {  
    string path = @"C:\ProgCS\Apêndice E\"; 

    StreamReader sr = new StreamReader(path + "regexlog.txt");
  
    XmlTextWriter xml = new XmlTextWriter(Console.Out);

    xml.Formatting = Formatting.Indented; 
    xml.WriteStartDocument();
    xml.WriteStartElement("Log");

    StringBuilder sbRegex = new StringBuilder();
    
    sbRegex.Append("(?<date>(\\d|\\/)+)\\s");
    sbRegex.Append("(?<time>(\\d|\\:)+)\\s");
    sbRegex.Append("(?<user>(\\S+))\\s");
    sbRegex.Append("(?<ip>(\\d|\\.)+)\\s");
    sbRegex.Append("(?<ws>(\\w+\\.\\w+)+)\\s");
    sbRegex.Append("(?<status>(\\d{3}))");

    Regex r = new Regex(sbRegex.ToString(), RegexOptions.IgnoreCase);

    string s;
    int count = 0;

    while((s = sr.ReadLine()) != null) 
    {
      Match m = r.Match(s);
      if(m.Success)
      {
        xml.WriteStartElement("Linha");
        xml.WriteAttributeString("Posição", Convert.ToString(++count));
        xml.WriteElementString("Data", m.Groups["date"].Value);
        xml.WriteElementString("Tempo", m.Groups["time"].Value);
        xml.WriteElementString("Usuário", m.Groups["user"].Value);
        xml.WriteElementString("IP", m.Groups["ip"].Value);
        xml.WriteElementString("WebService", m.Groups["ws"].Value);
        xml.WriteElementString("Status", m.Groups["status"].Value);
        xml.WriteEndElement();
      }    
    }

    sr.Close();

    xml.WriteEndElement();
    xml.WriteEndDocument();

    xml.Flush();
    xml.Close();
  }
}