//csc ArmazenadorDeInformações.cs

public class ArmazenadorDeInformações
{
  public System.Text.StringBuilder textointerno;
  
  public ArmazenadorDeInformações()
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
    ArmazenadorDeInformações info = new ArmazenadorDeInformações();
    info.Texto = "Isto é um teste.";
    info.Texto = "  Isto é outro teste.\n";
    info.Texto = "     Mais outro teste.    ";
    System.Console.WriteLine(info.Texto);
  }
}