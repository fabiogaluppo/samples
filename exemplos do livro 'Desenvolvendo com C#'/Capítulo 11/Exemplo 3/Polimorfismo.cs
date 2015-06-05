//csc Polimorfismo.cs

using System;

public interface IFunc
{
  void Func1();
}

public class ClasseA : IFunc
{
  public void Func1()
  {
    Console.WriteLine("Func1 ClasseA");
  }

  public static void Func2(IFunc f)
  {
    Console.Write("ClasseA Func2 = ");
    f.Func1();
  }
}

public class ClasseB : IFunc
{
  public void Func1()
  {
    Console.WriteLine("Func1 ClasseB");
  }  
}

public class Polimorfismo	
{
  public static void Main()
  {
    IFunc pf = new ClasseB();    
    pf.Func1();
    pf = new ClasseA();
    pf.Func1();

    ClasseA a = (ClasseA)pf; 
    a.Func1();

    ClasseB b;
    try
    {
      b = (ClasseB)pf; //InvalidCastException pf é concretizado pela ClasseA
    }
    catch(Exception e)
    {
      Console.WriteLine(e);
    }
    
    a = new ClasseA();
    b = new ClasseB();
    
    ClasseA.Func2(a);
    ClasseA.Func2(b);   
  }
}  
