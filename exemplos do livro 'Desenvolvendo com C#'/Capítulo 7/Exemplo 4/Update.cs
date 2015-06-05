//csc Update.cs

using System;
using System.Text;

public class Estudante
{
  #region Construtor
  public Estudante(){ alterado = Campos.nenhum; }
  #endregion  

  #region Campos privados
  private int id;
  private string nome;
  private string email;
  private string senha;
  private bool aprovado;
  #endregion
  
  [Flags]
  private enum Campos
  { 
    nenhum = 0x0,
    nome = 0x1, 
    email = 0x2, 
    senha = 0x4, 
    aprovado = 0x8
  }
    
  private Campos alterado; 

  #region Propriedades
  private int ID{ get{ return id; } set{ id = value; } }
  
  private string Nome
  { 
    get{ return nome; } 
    set{ nome = value; alterado |= Campos.nome; } 
  }

  private string Email
  { 
    get{ return email; } 
    set{ email = value; alterado |= Campos.email; } 
  }
  
  private string Senha
  { 
    get{ return senha; } 
    set{ senha = value; alterado |= Campos.senha; } 
  }
  
  private bool Aprovado
  { 
    get{ return aprovado; } 
    set{ aprovado = value; alterado |= Campos.aprovado; } 
  }
  #endregion

  #region Métodos
  public void Update()
  {
    if(alterado != Campos.nenhum)
    {
      StringBuilder sql = new StringBuilder("UPDATE ALUNOS SET ", 150);
				
      bool separador = false;
				
      if((alterado & Campos.nome) != 0)
      {
        sql.AppendFormat("NOME = '{0}'", nome);  separador = true;
      }
         
      if((alterado & Campos.email) != 0)
      {
        ConcatSQL(sql, "EMAIL", email, separador, true);
        separador = true;
      }

      if((alterado & Campos.senha) != 0)
      {
        ConcatSQL(sql, "SENHA", senha, separador, true);
        separador = true;
      }

      if((alterado & Campos.aprovado) != 0)
      {
        ConcatSQL(sql, "APROVADO", aprovado? 1 : 0, separador, false);
      }

      sql.AppendFormat(" WHERE ID = {0}", id);

      Console.WriteLine("{0}\n", sql.ToString());
    }
  }

  void ConcatSQL(StringBuilder sb, string cp, object vl, bool sep, bool asp)
  {
    if(sep) sb.Append(", ");
    sb.AppendFormat((asp? "{0} = '{1}'" : "{0} = {1}"), cp, vl);
  }
  #endregion

  public static void Main()
  {
    Estudante est1 = new Estudante();
    est1.ID = 100; 
    est1.Nome = "Fabio Galuppo"; 
    est1.Email = "fabiogaluppo@hotmail.com";
    est1.Update();
       
    Estudante est2 = new Estudante();
    est2.ID = 150; 
    est2.Nome = "Wallace Santos"; 
    est2.Senha = "Teste.123_456";
    est2.Update();

    Estudante est3 = new Estudante();
    est3.ID = 168; 
    est3.Nome = "Vanclei Matheus"; 
    est3.Senha = "XYZW.000&123";
    est3.Email = "vanclei@desenvolvendo.net";
    est3.Aprovado = true;
    est3.Update();

    Estudante est4 = new Estudante();
    est4.ID = 175; 
    est4.Senha = "FIREBALL";
    est4.Aprovado = false;
    est4.Update();
  }  
}

