//csc Catálogo.cs
//csc Catálogo.cs /define:MontarListaViaReflection

using System;
using System.Collections;

#if MontarListaViaReflection
using System.Reflection;
#endif

public enum Item{Livro, CD};

public interface ICatálogo
{
  string[] Listar(Item item);  
}

public interface ICatálogoLivro
{
  Livro ObterInfo(string id);	
}

public interface ICatálogoCD
{
  CD ObterInfo(string id);
}

public class Livro 
{
  public string Título, Autor;
}

public class CD
{
  public string Título, Banda;
} 

public class CDComparer : IComparer
{
  public int Compare(object o1, object o2)
  {
    CD cd = (CD)o1;
    string título = (string)o2;
    
    int l1 = cd.Título.Length;
    int l2 = título.Length;
		  
    if(l1 > l2)
      return 1;
    else if(l1 < l2)
      return -1;
    return cd.Título.CompareTo(título);
  }
}

public class LivroComparer : IComparer
{
  public int Compare(object o1, object o2)
  {
    Livro livro = (Livro)o1;
    string título = (string)o2;
    
    int l1 = livro.Título.Length;
    int l2 = título.Length;
		  
    if(l1 > l2)
      return 1;
    else if(l1 < l2)
      return -1;
    return livro.Título.CompareTo(título);
  }
}

public class CatálogoManager : ICatálogo, ICatálogoLivro, ICatálogoCD
{
  static private Livro[] livros;
  static  private CD[] cds;
  static private string[] _cds, _livros;

  public const int NúmeroDeLivros = 100;
  public const int NúmeroDeCDs = 100;

  static CatálogoManager()
  {
    livros = new Livro[NúmeroDeLivros];
    cds = new CD[NúmeroDeCDs];

    for(int a = 0; a < NúmeroDeLivros; ++a)
    {
      livros[a] = new Livro();
      livros[a].Título = String.Format("{0} {1}", "Livro", a);
      livros[a].Autor = String.Format("{0} {1}", "Autor", a);;
    }
    
    for(int a = 0; a < NúmeroDeCDs; ++a)
    {
      cds[a] = new CD();
      cds[a].Título = String.Format("{0} {1}", "CD", a);;
      cds[a].Banda = String.Format("{0} {1}", "Autor", a);;
    }		
  }

  Livro ICatálogoLivro.ObterInfo(string id)
  {
    int i = Array.BinarySearch(livros, id, new LivroComparer());
    
    if(i < 0) throw new ApplicationException("Livro não encontrado");
    return livros[i];
  }

  CD ICatálogoCD.ObterInfo(string id)
  {
    int i = Array.BinarySearch(cds, id, new CDComparer());
    
    if(i < 0) throw new ApplicationException("CD não encontrado");    
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

  string[] ICatálogo.Listar(Item item)
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
            _cds[a] = cds[a].Título; 
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
            _livros[a] = livros[a].Título; 
          #endif
        }
        
        return _livros;
    }
		
    return null; 
  }

  public static void Main()
  {
    ICatálogoLivro ctlv = (ICatálogoLivro)new CatálogoManager();
    ICatálogo c = (ICatálogo)ctlv;
    string[] lista = c.Listar(Item.Livro);

    int idx = new Random().Next(0, CatálogoManager.NúmeroDeLivros - 1);
    Livro livro = ctlv.ObterInfo(lista[idx]);

    Console.WriteLine("{0} - {1}", livro.Título, livro.Autor);

    lista = c.Listar(Item.CD);

    ICatálogoCD ctcd = (ICatálogoCD)c;
		
    idx = new Random().Next(0, CatálogoManager.NúmeroDeCDs - 1);
    CD cd = ctcd.ObterInfo(lista[idx]);

    Console.WriteLine("{0} - {1}", cd.Título, cd.Banda);
  }
}
