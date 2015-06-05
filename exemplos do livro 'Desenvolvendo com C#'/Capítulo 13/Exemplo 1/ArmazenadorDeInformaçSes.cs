//csc ArmazenadorDeInforma��es.cs

public class ArmazenadorDeInforma��es
{
  public System.Text.StringBuilder textointerno;
  
  public ArmazenadorDeInforma��es()
  {
    textointerno = new System.Text.StringBuilder(200);
  }
  
  public string Texto
  {
    get{ return textointerno.ToString(); }

    set
    {
      if(value != null)
      {
        string fmt;
        if(textointerno.Length > 0) fmt = " {0}"; else fmt = "{0}";
        textointerno.AppendFormat(fmt, value.Trim());
      }
    }
  }

  public static void Main()
  {
    ArmazenadorDeInforma��es info = new ArmazenadorDeInforma��es();
    info.Texto = "Isto � um teste.";
    info.Texto = "  Isto � outro teste.\n";
    info.Texto = "     Mais outro teste.    ";
    System.Console.WriteLine(info.Texto);
  }
}