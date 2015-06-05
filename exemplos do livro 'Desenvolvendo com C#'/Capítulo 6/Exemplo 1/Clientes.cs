//csc Clientes.cs

namespace MinhaEmpresa
{
  public class Cliente
  {
    public string Nome;
    public string Telefone;

    public void Salvar(){} 
  }
}

namespace Acme
{
  public class Cliente
  {
    public Cliente(string ID){}

    public string Nome;
    public string Telefone; 
  }
}

public class Clientes
{
  public static void Main()
  {
    Acme.Cliente acmeCliente = new Acme.Cliente("ID-12345");
    MinhaEmpresa.Cliente meuCliente = new MinhaEmpresa.Cliente();
    meuCliente.Nome = acmeCliente.Nome;
    meuCliente.Telefone = meuCliente.Telefone;
    
    meuCliente.Salvar();
  }
}
