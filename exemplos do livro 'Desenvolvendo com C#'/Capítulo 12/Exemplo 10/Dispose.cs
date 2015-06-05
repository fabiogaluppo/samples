//csc Dispose.cs WinApiHelper.cs

using System;

public class CGlobalAtomWrapper : IDisposable
{
  private int m_Size;
  private short m_Atom;
  private bool m_Disposed;

  public CGlobalAtomWrapper(string s)
  {
    m_Size = s.Length;	
    m_Atom = WinAPIHelper.GlobalAddAtom(s);
    m_Disposed = false;    
  }

  public string GetData()
  {	
    char[] s = new char[m_Size];
    WinAPIHelper.GlobalGetAtomName(m_Atom, s, m_Size);
    return new string(s);
  }

  ~CGlobalAtomWrapper()
  {
    FreeResources();
  }

  public void Dispose()
  {
    FreeResources();
  }

  public void Close() //método amigável :)
  {
    Dispose();
  }

  void FreeResources()
  {	
    if(!m_Disposed)
    {      
      if(m_Atom != 0)
	WinAPIHelper.GlobalDeleteAtom(m_Atom);

      //Se a classe implementa IDisposable:
      //base.Dispose();

      GC.SuppressFinalize(this);

      m_Disposed = true;      
    }	
  }
}

public class MyApp
{
  public static void Main()
  {
    CGlobalAtomWrapper GlobalAtom = new CGlobalAtomWrapper("Hello World");
    Console.WriteLine(GlobalAtom.GetData());

    CGlobalAtomWrapper GlobalAtom2 = new CGlobalAtomWrapper("This is C#!!!");
    Console.WriteLine(GlobalAtom2.GetData());
  }
}
