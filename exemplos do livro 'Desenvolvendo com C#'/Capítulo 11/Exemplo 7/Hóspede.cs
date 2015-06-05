//csc /target:library Hóspede.cs /r:System.dll
 
using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Configuration;

[assembly: AssemblyTitle("Componente Hóspede")]
[assembly: AssemblyDescription("Componente para manipulação de hóspedes")]
[assembly: AssemblyCompany("Desenvolvendo.NET S.A.")]
[assembly: AssemblyProduct("Sistema de hotelaria")]

//Versionamento
[assembly: AssemblyVersion("1.0.0.0")]

/// <summary>
/// Representa o tipo Hóspede
/// </summary>
public class Hóspede
{
  private int id;
  private string nome, sobrenome, email;
  
  /// <summary>
  /// Identificação do hóspede
  /// </summary>
  public int ID{ get{ return id; } set{ id = value; } }
  
  /// <summary>
  /// Nome do hóspede
  /// </summary>
  public string Nome{ get{ return nome; } set{ nome = value; } }

  /// <summary>
  /// Sobrenome do hóspede
  /// </summary>
  public string Sobrenome{ get{ return sobrenome; } set{ sobrenome = value; } }
  
  /// <summary>
  /// Email do hóspede
  /// </summary>
  public string Email{ get{ return email; } set{ email = value; } }  
}

/// <summary>
/// Classe abstrata para procura de informações do hóspede
/// </summary>
public abstract class HóspedeFinder
{
  /// <summary>
  /// Busca hóspede pelo Email
  /// </summary>
  /// <param name="email">Email do hóspede</param>
  /// <returns>Instância do hóspede encontrado, ou null se não encontrado</returns>
  public abstract Hóspede ProcurarPeloEmail(string email);
}

/// <summary>
/// Classe para procurar de informações do hóspede (Simples)
/// </summary>
public class HóspedeSimpleFinder : HóspedeFinder
{
  /// <summary>
  /// Busca hóspede pelo Email
  /// </summary>
  /// <param name="email"></param>
  /// <returns>Instância do hóspede encontrado, ou null se não encontrado</returns>
  public override Hóspede ProcurarPeloEmail(string email)
  {
    XmlTextReader reader = new XmlTextReader(ConfigurationSettings.AppSettings["Xml Source"]);
    
    Hóspede h = null;

    while(reader.Read())
    {
      if(reader["Email"] == email)
      {
        h = new Hóspede(); 
        h.Email = email;
        h.Nome = reader["Nome"];
        h.Sobrenome = reader["Sobrenome"];
        h.ID = Convert.ToInt32(reader["ID"]);
      }
    }    
    reader.Close();

    return h;
  }
}