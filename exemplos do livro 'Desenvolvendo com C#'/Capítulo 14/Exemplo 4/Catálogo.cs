//csc Cat�logo.cs
//csc Cat�logo.cs /define:MontarListaViaReflection

using System;
using System.Collections;

#if MontarListaViaReflection
using System.Reflection;
#endif

public enum Item{Livro, CD};

public interface ICat�logo
{
  string[] Listar(Item item);  
}

public interface ICat�logoLivro
{
  Livro ObterInfo(string id);	
}

public interface ICat�logoCD
{
  CD ObterInfo(string id);
}

public class Livro 
{
  public string T�tulo, Autor;
}

public class CD
{
  public string T�tulo, Banda;
} 

public class CDComparer : IComparer
{
  public int Compare(object o1, object o2)
  {
    CD cd = (CD)o1;
    string t�tulo = (string)o2;
    
    int l1 = cd.T�tulo.Length;
    int l2 = t�tulo.Length;
		  
    if(l1 > l2)
      return 1;
    else if(l1 < l2)
      return -1;
    return cd.T�tulo.CompareTo(t�tulo);
  }
}

public class LivroComparer : IComparer
{
  public int Compare(object o1, object o2)
  {
    Livro livro = (Livro)o1;
    string t�tulo = (string)o2;
    
    int l1 = livro.T�tulo.Length;
    int l2 = t�tulo.Length;
		  
    if(l1 > l2)
      return 1;
    else if(l1 < l2)
      return -1;
    return livro.T�tulo.CompareTo(t�tulo);
  }
}

public class Cat�logoManager : ICat�logo, ICat�logoLivro, ICat�logoCD
{
  static private Livro[] livros;
  static  private CD[] cds;
  static private string[] _cds, _livros;

  public const int N�meroDeLivros = 100;
  public const int N�meroDeCDs = 100;

  static Cat�logoManager()
  {
    livros = new Livro[N�meroDeLivros];
    cds = new CD[N�meroDeCDs];

    for(int a = 0; a < N�meroDeLivros; ++a)
    {
      livros[a] = new Livro();
      livros[a].T�tulo = String.Format("{0} {1}", "Livro", a);
      livros[a].Autor = String.Format("{0} {1}", "Autor", a);;
    }
    
    for(int a = 0; a < N�meroDeCDs; ++a)
    {
      cds[a] = new CD();
      cds[a].T�tulo = String.Format("{0} {1}", "CD", a);;
      cds[a].Banda = String.Format("{0} {1}", "Autor", a);;
    }		
  }

  Livro ICat�logoLivro.ObterInfo(string id)
  {
    int i = Array.BinarySearch(livros, id, new LivroComparer());
    
    if(i < 0) throw new ApplicationException("Livro n�o encontrado");
    return livros[i];
  }

  CD ICat�logoCD.ObterInfo(string id)
  {
    int i = Array.BinarySearch(cds, id, new CDComparer());
    
    if(i < 0) throw new ApplicationException("CD n�o encontrado");    
    return cds[i];
  }

#if MontarListaViaReflection
  private string[] MontarLista(Type t, object[] instances)
  {
    FieldInfo[] flds = t.GetFields();
		
    int l = instances.Length;
    string[] vetor = new string[l];

    for(int a = 0; a < l; ++a)
      vetor[a] = (string)flds[0].GetValue(instances[a]);
		
    return vetor;
  }
#endif

  string[] ICat�logo.Listar(Item item)
  { 
    switch(item)
    {
      case Item.CD:
	if(_cds == null)
        {
	  #if MontarListaViaReflection
          _cds = MontarLista(typeof(CD), cds);				
	  #else
          int l = cds.Length;
          _cds = new string[l];
          for(int a = 0; a < l; ++a)
            _cds[a] = cds[a].T�tulo; 
          #endif
        }  
        return _cds;
      case Item.Livro:
        if(_livros == null)
        {
          #if MontarListaViaReflection 
          _livros = MontarLista(typeof(Livro), livros);
          #else
          int l = livros.Length;
          _livros = new string[l];
          for(int a = 0; a < l; ++a)
            _livros[a] = livros[a].T�tulo; 
          #endif
        }
        
        return _livros;
    }
		
    return null; 
  }

  public static void Main()
  {
    ICat�logoLivro ctlv = (ICat�logoLivro)new Cat�logoManager();
    ICat�logo c = (ICat�logo)ctlv;
    string[] lista = c.Listar(Item.Livro);

    int idx = new Random().Next(0, Cat�logoManager.N�meroDeLivros - 1);
    Livro livro = ctlv.ObterInfo(lista[idx]);

    Console.WriteLine("{0} - {1}", livro.T�tulo, livro.Autor);

    lista = c.Listar(Item.CD);

    ICat�logoCD ctcd = (ICat�logoCD)c;
		
    idx = new Random().Next(0, Cat�logoManager.N�meroDeCDs - 1);
    CD cd = ctcd.ObterInfo(lista[idx]);

    Console.WriteLine("{0} - {1}", cd.T�tulo, cd.Banda);
  }
}
