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
    //Isto � apenas um exemplo, o banco de dados n�o est� sendo usado
    
    Console.WriteLine("Persistir: {0} {1}", nomeCorrentista, valor);    
  }
}
