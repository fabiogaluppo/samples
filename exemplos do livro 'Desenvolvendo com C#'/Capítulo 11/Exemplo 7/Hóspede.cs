//csc /target:library H�spede.cs /r:System.dll
 
using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Configuration;

[assembly: AssemblyTitle("Componente H�spede")]
[assembly: AssemblyDescription("Componente para manipula��o de h�spedes")]
[assembly: AssemblyCompany("Desenvolvendo.NET S.A.")]
[assembly: AssemblyProduct("Sistema de hotelaria")]

//Versionamento
[assembly: AssemblyVersion("1.0.0.0")]

/// <summary>
/// Representa o tipo H�spede
/// </summary>
public class H�spede
{
  private int id;
  private string nome, sobrenome, email;
  
  /// <summary>
  /// Identifica��o do h�spede
  /// </summary>
  public int ID{ get{ return id; } set{ id = value; } }
  
  /// <summary>
  /// Nome do h�spede
  /// </summary>
  public string Nome{ get{ return nome; } set{ nome = value; } }

  /// <summary>
  /// Sobrenome do h�spede
  /// </summary>
  public string Sobrenome{ get{ return sobrenome; } set{ sobrenome = value; } }
  
  /// <summary>
  /// Email do h�spede
  /// </summary>
  public string Email{ get{ return email; } set{ email = value; } }  
}

/// <summary>
/// Classe abstrata para procura de informa��es do h�spede
/// </summary>
public abstract class H�spedeFinder
{
  /// <summary>
  /// Busca h�spede pelo Email
  /// </summary>
  /// <param name="email">Email do h�spede</param>
  /// <returns>Inst�ncia do h�spede encontrado, ou null se n�o encontrado</returns>
  public abstract H�spede ProcurarPeloEmail(string email);
}

/// <summary>
/// Classe para procurar de informa��es do h�spede (Simples)
/// </summary>
public class H�spedeSimpleFinder : H�spedeFinder
{
  /// <summary>
  /// Busca h�spede pelo Email
  /// </summary>
  /// <param name="email"></param>
  /// <returns>Inst�ncia do h�spede encontrado, ou null se n�o encontrado</returns>
  public override H�spede ProcurarPeloEmail(string email)
  {
    XmlTextReader reader = new XmlTextReader(ConfigurationSettings.AppSettings["Xml Source"]);
    
    H�spede h = null;

    while(reader.Read())
    {
      if(reader["Email"] == email)
      {
        h = new H�spede(); 
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