//csc /t:library Conta.cs

using System;
using System.EnterpriseServices;
using System.Reflection;

[assembly: ApplicationName("ContaCorrente")]

[assembly: AssemblyKeyFile("key.snk")]
 
[Transaction]
public class Conta : ServicedComponent 
{
  [AutoComplete]
  public void Creditar(string nomeCorrentista, double valor)
  {
    //Isto é apenas um exemplo, o banco de dados não está sendo usado
    
    Console.WriteLine("Persistir: {0} {1}", nomeCorrentista, valor);    
  }
}
